using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

///<summary>Tracks effect repetitions for each native CardPlay.</summary>
public static class DeviousEffectLoopTracker
{
    public sealed class EffectState
    {
        public bool Initialized;
        public int RepeatCount;
        public int Iteration;

        internal CardPlay? CardPlay;
        internal EnchantmentModel? Enchantment;
        internal AfflictionModel? Affliction;
        internal EnchantmentStatus InitialEnchantmentStatus;
        internal bool AttachmentsCaptured;
        internal readonly List<CapturedValue?> BlockCaptures = [];
        internal readonly List<CapturedValue?> DamageCaptures = [];
        internal readonly List<CapturedValue?> ModifierBlockCaptures = [];
        internal readonly List<CapturedValue?> ModifierDamageCaptures = [];
        internal int BlockCursor;
        internal int DamageCursor;
        internal int ModifierBlockCursor;
        internal int ModifierDamageCursor;
        internal int ModifierIteration;
        internal bool InModifierEffects;
        internal EffectState? PreviousCurrent;
    }

    ///<summary>A resolved damage or Block hook calculation. (e.g. with Vambrace or Pen Nib)</summary>
    public sealed class CapturedValue
    {
        internal object? Target;
        internal object? Dealer;
        internal object? CardSource;
        internal decimal Input;
        internal decimal Result;
        internal ValueProp Props;
        internal ModifyDamageHookType DamageHookType;
        internal AbstractModel[] Modifiers = [];

        internal bool Matches(CapturedValue other, bool damage)
        {
            if (Input != other.Input || Props != other.Props || !ReferenceEquals(Target, other.Target) || !ReferenceEquals(CardSource, other.CardSource)) return false;

            return !damage || (DamageHookType == other.DamageHookType && ReferenceEquals(Dealer, other.Dealer));
        }
    }

    ///<summary>Traces initial calculation of a prefix to its postfix.</summary>
    public sealed class CaptureToken
    {
        internal CaptureToken(EffectState state, int index, CapturedValue value, bool damage)
        {
            State = state;
            Index = index;
            Value = value;
            Damage = damage;
        }

        internal EffectState State { get; }
        internal int Index { get; }
        internal CapturedValue Value { get; }
        internal bool Damage { get; }
    }

    private static readonly ConditionalWeakTable<CardPlay, EffectState> States = new();
    private static readonly AsyncLocal<EffectState?> Current = new();

    public static EffectState GetState(CardPlay cardPlay)
    {
        var state = States.GetOrCreateValue(cardPlay);
        state.CardPlay ??= cardPlay;
        CaptureAttachments(state);
        return state;
    }

    ///<summary>Manually append the Thumbscrew hit</summary>
    internal static bool ShouldAddThumbscrewDamageHit(CardPlay cardPlay)
        => States.TryGetValue(cardPlay, out var state)
           && state is { Initialized: true, RepeatCount: > 1 }
           && state.Iteration == state.RepeatCount - 1
           && cardPlay.Card.Type is CardType.Attack
           && cardPlay.Card.Owner.Relics.OfType<Thumbscrew>().Any();

    ///<summary>Starts an effect iteration in OnPlayWrapper, covering the card, enchantment and affliction.</summary>
    public static void BeginEffectIteration(CardPlay cardPlay)
    {
        var state = GetState(cardPlay);
        state.InModifierEffects = false;
        state.BlockCursor = 0;
        state.DamageCursor = 0;
        SetCurrent(state);
    }

    ///<summary>Starts a pass through BaseLib's CardModifier.OnPlay dispatcher.</summary>
    public static void BeginModifierEffects(CardPlay cardPlay)
    {
        if (!States.TryGetValue(cardPlay, out var state) || !state.Initialized) return;
        state.InModifierEffects = true;
        state.ModifierBlockCursor = 0;
        state.ModifierDamageCursor = 0;
        if (state.RepeatCount > 0) SetCurrent(state);
    }

    ///<summary>Only skip BaseLib CardModifier.OnPlay effects when Devious is 0.</summary>
    public static bool ShouldSkipModifierEffects(CardPlay cardPlay) => States.TryGetValue(cardPlay, out var state) && state is { Initialized: true, RepeatCount: <= 0 };

    ///<summary>Loop BaseLib's modifiers, ignoring Refund/Purge and external global hooks.</summary>
    ///Note: Need to make this multiply Refund eventually, don't know any mods that use it atm.
    public static bool ShouldRepeatModifierEffects(CardPlay cardPlay)
    {
        if (!States.TryGetValue(cardPlay, out var state) || !state.Initialized || state.RepeatCount <= 0)
        {
            if (state is null) return false;
            state.InModifierEffects = false;
            ClearCurrent(state);
            return false;
        }

        if (state.ModifierIteration >= state.RepeatCount - 1)
        {
            state.ModifierIteration = state.RepeatCount;
            state.InModifierEffects = false;
            ClearCurrent(state);
            return false;
        }

        state.ModifierIteration++;
        return true;
    }

    ///<summary>Skip the remaining native effects when Devious is 0.</summary>
    public static bool ShouldSkipEffects(CardPlay cardPlay)
    {
        var skip = States.TryGetValue(cardPlay, out var state) && state is { Initialized: true, RepeatCount: <= 0 };
        if (skip && state is not null) ClearCurrent(state);
        return skip;
    }

    ///<summary>After native effects, repeat with the same CardPlay and a fresh choice context if needed.</summary>
    public static bool ShouldRepeat(CardPlay cardPlay)
    {
        if (!States.TryGetValue(cardPlay, out var state) || !state.Initialized || state.RepeatCount <= 0)
        {
            if (state is not null) ClearCurrent(state);
            return false;
        }

        if (state.Iteration >= state.RepeatCount - 1)
        {
            state.Iteration = state.RepeatCount;
            ClearCurrent(state);
            return false;
        }

        state.Iteration++;
        return true;
    }

    internal static bool ShouldShowEnchantmentAsNormal(EnchantmentModel enchantment, EnchantmentStatus actualStatus)
    {
        var state = Current.Value;
        return actualStatus is EnchantmentStatus.Disabled
               && state is { Initialized: true, RepeatCount: > 0, InModifierEffects: false, InitialEnchantmentStatus: EnchantmentStatus.Normal }
               && ReferenceEquals(state.Enchantment, enchantment);
    }

    /// <summary>Oh, God! Why is tainted hardcoded??? CASSSEEYYYYYYYY!!!!!!!</summary>
    internal static bool ShouldRunTaintedEffects(AfflictionModel affliction)
    {
        var state = Current.Value;
        return state is { Initialized: true, RepeatCount: > 0, InModifierEffects: false } && ReferenceEquals(state.Affliction, affliction) && ReferenceEquals(state.CardPlay?.Card.Affliction, affliction);
    }

    internal static bool ShouldSuppressNativeTaintedAfterCardPlayed(CardPlay cardPlay) => States.TryGetValue(cardPlay, out var state) && state is { Initialized: true, Affliction: Tainted };

    internal static CaptureToken? PrepareBlockCapture(CardPlay? cardPlay, CardModel? cardSource, Creature target, decimal input, ValueProp props, out CapturedValue? captured) 
    {
        captured = null;
        var state = ActiveStateFor(cardPlay, cardSource);
        if (state is null) return null;

        var modifierPass = state.InModifierEffects;
        var index = modifierPass ? state.ModifierBlockCursor++ : state.BlockCursor++;
        var captures = modifierPass ? state.ModifierBlockCaptures : state.BlockCaptures;
        var iteration = modifierPass ? state.ModifierIteration : state.Iteration;
        var candidate = new CapturedValue
        {
            Target = target,
            CardSource = cardSource,
            Input = input,
            Props = props
        };

        if (iteration <= 0
            || index >= captures.Count
            || captures[index] is not { } existing
            || !existing.Matches(candidate, damage: false))
            return iteration is 0 ? new CaptureToken(state, index, candidate, damage: false) : null;
        captured = existing;
        return null;
    }

    internal static CaptureToken? PrepareDamageCapture(CardPlay? cardPlay, CardModel? cardSource, Creature? target, Creature? dealer, decimal input, ValueProp props, ModifyDamageHookType hookType, CardPreviewMode previewMode, out CapturedValue? captured)
    {
        captured = null;
        if (previewMode is not CardPreviewMode.None) return null;

        var state = ActiveStateFor(cardPlay, cardSource);
        if (state is null) return null;

        var modifierPass = state.InModifierEffects;
        var index = modifierPass ? state.ModifierDamageCursor++ : state.DamageCursor++;
        var captures = modifierPass ? state.ModifierDamageCaptures : state.DamageCaptures;
        var iteration = modifierPass ? state.ModifierIteration : state.Iteration;
        var candidate = new CapturedValue
        {
            Target = target,
            Dealer = dealer,
            CardSource = cardSource,
            Input = input,
            Props = props,
            DamageHookType = hookType
        };

        if (iteration <= 0
            || index >= captures.Count
            || captures[index] is not { } existing
            || !existing.Matches(candidate, damage: true))
            return iteration is 0 ? new CaptureToken(state, index, candidate, damage: true) : null;
        captured = existing;
        return null;
    }

    internal static void CompleteCapture(CaptureToken? token, decimal result, IEnumerable<AbstractModel> modifiers)
    {
        if (token is null) return;

        token.Value.Result = result;
        token.Value.Modifiers = modifiers?.ToArray() ?? [];
        var captures = token.State.InModifierEffects
            ? token.Damage ? token.State.ModifierDamageCaptures : token.State.ModifierBlockCaptures
            : token.Damage ? token.State.DamageCaptures : token.State.BlockCaptures;
        while (captures.Count <= token.Index) captures.Add(null);
        captures[token.Index] = token.Value;
    }

    private static EffectState? ActiveStateFor(CardPlay? cardPlay, CardModel? cardSource)
    {
        var state = Current.Value;
        if (state is not { Initialized: true, RepeatCount: > 0 } || state.CardPlay is null) return null;

        if (cardPlay is not null) return ReferenceEquals(cardPlay, state.CardPlay) ? state : null;

        //Match card-sourced calculations that don't receive a CardPlay.
        return cardSource is not null && ReferenceEquals(cardSource, state.CardPlay.Card) ? state : null;
    }

    private static void CaptureAttachments(EffectState state)
    {
        if (state.AttachmentsCaptured || state.CardPlay is null) return;
        state.AttachmentsCaptured = true;
        state.Enchantment = state.CardPlay.Card.Enchantment;
        state.Affliction = state.CardPlay.Card.Affliction;
        state.InitialEnchantmentStatus = state.Enchantment?.Status ?? EnchantmentStatus.Disabled;
    }

    private static void SetCurrent(EffectState state)
    {//Loops must not set their own new branch, that should only happen for nested cardplays (e.g. Devious discarding Sly)
        if (!ReferenceEquals(Current.Value, state)) state.PreviousCurrent = Current.Value;
        Current.Value = state;
    }

    private static void ClearCurrent(EffectState state)
    {
        if (!ReferenceEquals(Current.Value, state)) return;
        Current.Value = state.PreviousCurrent;
        state.PreviousCurrent = null;
    }

    public static void ClearAll()
    {
        Current.Value = null;
        States.Clear();
    }
}

///<summary>Repeats native card, enchantment and affliction effects within one CardPlay.
///BeforeCardPlayed, AfterCardPlayed and history remain outside this loop; BaseLib modifiers loop separately.</summary>
[HarmonyPatch]
public static class DeviousEffectLoopPatch
{
    [HarmonyPrepare]
    public static bool Prepare() => TargetMethod() is not null;

    [HarmonyTargetMethod]
    public static MethodBase? TargetMethod()
    {
        var original = AccessTools.Method(typeof(CardModel), nameof(CardModel.OnPlayWrapper));
        if (original is null) return null;

        var asyncAttribute = original.GetCustomAttribute<AsyncStateMachineAttribute>();
        return asyncAttribute is null ? original : AccessTools.Method(asyncAttribute.StateMachineType, "MoveNext");
    }

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original, ILGenerator generator)
    {
        var list = instructions.ToList();
        var stateMachineType = original.DeclaringType;
        var cardPlayField = stateMachineType?.GetFields(AccessTools.all).FirstOrDefault(f => f.FieldType == typeof(CardPlay) && f.Name.Contains("cardPlay"));
        var choiceContextField = stateMachineType?.GetFields(AccessTools.all).FirstOrDefault(f => f.Name.Contains("choiceContext"));
        var effectCombatIdField = stateMachineType?.GetFields(AccessTools.all).FirstOrDefault(f => f.Name.Contains("effectCombatId"));
        if (cardPlayField is null || choiceContextField is null || effectCombatIdField is null) return list;

        var onPlayCall = list.FindIndex(i => IsMethod(i, "OnPlay", nameof(CardModel)));
        var contextConstructor = onPlayCall < 0
            ? -1
            : list.FindIndex(i => i.opcode == OpCodes.Newobj && IsMethod(i, ".ctor", "BranchingPlayerChoiceContext"));
        var executionFinished = onPlayCall < 0
            ? -1
            : list.FindIndex(onPlayCall + 1, i => IsMethod(i, "InvokeExecutionFinished", "AbstractModel"));
        var historyFinished = executionFinished < 0
            ? -1
            : list.FindIndex(executionFinished + 1, i => IsMethod(i, "CardPlayFinished", "CombatHistory"));
        var isCurrentLiveCombat = historyFinished < 0
            ? -1
            : list.FindLastIndex(historyFinished - 1, i => IsMethod(i, "IsCurrentLiveCombat", "CombatManager"));

        var loopStartIndex = contextConstructor - 7;
        var skipInsertIndex = executionFinished - 1;
        var historyInsertIndex = isCurrentLiveCombat - 3;
        if (onPlayCall < 0
            || !IsChoiceContextSetup(list, loopStartIndex, contextConstructor, choiceContextField)
            || !IsExecutionFinishedSetup(list, skipInsertIndex, executionFinished)
            || !IsHistorySetup(list, historyInsertIndex, isCurrentLiveCombat, effectCombatIdField)
            || loopStartIndex >= skipInsertIndex
            || skipInsertIndex >= historyInsertIndex)
        {
            return list;
        }

        var loopStartLabel = generator.DefineLabel();
        var skipToHistoryLabel = generator.DefineLabel();

        var beginIteration = AccessTools.Method(
            typeof(DeviousEffectLoopTracker), nameof(DeviousEffectLoopTracker.BeginEffectIteration));
        var shouldSkip = AccessTools.Method(
            typeof(DeviousEffectLoopTracker), nameof(DeviousEffectLoopTracker.ShouldSkipEffects));
        var shouldRepeat = AccessTools.Method(
            typeof(DeviousEffectLoopTracker), nameof(DeviousEffectLoopTracker.ShouldRepeat));
        if (beginIteration is null || shouldSkip is null || shouldRepeat is null) return list;

        //Insert later IL changes first so earlier indices don’t shift. Moving labels ensures existing branches still reach the new repeat check.
        var loopback = new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, cardPlayField),
            new CodeInstruction(OpCodes.Call, shouldRepeat),
            new CodeInstruction(OpCodes.Brtrue, loopStartLabel),
        };
        var historyInstruction = list[historyInsertIndex];
        MoveLabels(historyInstruction, loopback[0]);
        //When Devious is 0, jump straight to history instead of looping. 
        historyInstruction.labels.Add(skipToHistoryLabel);
        list.InsertRange(historyInsertIndex, loopback);

        var skip = new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, cardPlayField),
            new CodeInstruction(OpCodes.Call, shouldSkip),
            new CodeInstruction(OpCodes.Brtrue, skipToHistoryLabel),
        };
        MoveLabels(list[skipInsertIndex], skip[0]);
        list.InsertRange(skipInsertIndex, skip);

        //Set AsyncLocal in the wrapper, not only inside the card’s OnPlay, so it remains available to the succeeding enchantment and affliction calls.
        var begin = new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, cardPlayField),
            new CodeInstruction(OpCodes.Call, beginIteration),
        };
        MoveLabels(list[loopStartIndex], begin[0]);
        begin[0].labels.Add(loopStartLabel);
        list.InsertRange(loopStartIndex, begin);

        return list;
    }

    private static bool IsChoiceContextSetup(IReadOnlyList<CodeInstruction> instructions, int start, int constructor, FieldInfo choiceContextField)
    {
        if (start < 0 || constructor != start + 7 || constructor >= instructions.Count) return false;
        return IsLdloc(instructions[start])
               && IsMethod(instructions[start + 1], "get_NetId", "LocalContext")
               && IsStloc(instructions[start + 2])
               && IsLdloca(instructions[start + 3])
               && IsMethod(instructions[start + 4], "get_Value", "Nullable`1")
               && instructions[start + 5].opcode == OpCodes.Ldarg_0
               && IsFieldLoad(instructions[start + 6], choiceContextField)
               && instructions[constructor].opcode == OpCodes.Newobj
               && IsMethod(instructions[constructor], ".ctor", "BranchingPlayerChoiceContext");
    }

    private static bool IsExecutionFinishedSetup(IReadOnlyList<CodeInstruction> instructions, int start, int executionFinished)
        => start >= 0
           && executionFinished == start + 1
           && executionFinished < instructions.Count
           && IsLdloc(instructions[start])
           && IsMethod(instructions[executionFinished], "InvokeExecutionFinished", "AbstractModel");

    private static bool IsHistorySetup(IReadOnlyList<CodeInstruction> instructions, int start, int isCurrentLiveCombat, FieldInfo effectCombatIdField)
        => start >= 0
           && isCurrentLiveCombat == start + 3
           && isCurrentLiveCombat < instructions.Count
           && IsMethod(instructions[start], "get_Instance", "CombatManager")
           && instructions[start + 1].opcode == OpCodes.Ldarg_0
           && IsFieldLoad(instructions[start + 2], effectCombatIdField)
           && IsMethod(instructions[isCurrentLiveCombat], "IsCurrentLiveCombat", "CombatManager");

    private static bool IsLdloc(CodeInstruction instruction)
        => instruction.opcode == OpCodes.Ldloc
           || instruction.opcode == OpCodes.Ldloc_S
           || instruction.opcode == OpCodes.Ldloc_0
           || instruction.opcode == OpCodes.Ldloc_1
           || instruction.opcode == OpCodes.Ldloc_2
           || instruction.opcode == OpCodes.Ldloc_3;

    private static bool IsStloc(CodeInstruction instruction)
        => instruction.opcode == OpCodes.Stloc
           || instruction.opcode == OpCodes.Stloc_S
           || instruction.opcode == OpCodes.Stloc_0
           || instruction.opcode == OpCodes.Stloc_1
           || instruction.opcode == OpCodes.Stloc_2
           || instruction.opcode == OpCodes.Stloc_3;

    private static bool IsLdloca(CodeInstruction instruction) => instruction.opcode == OpCodes.Ldloca || instruction.opcode == OpCodes.Ldloca_S;

    private static bool IsFieldLoad(CodeInstruction instruction, FieldInfo field) => instruction.opcode == OpCodes.Ldfld && Equals(instruction.operand, field);

    private static void MoveLabels(CodeInstruction source, CodeInstruction destination)
    {
        if (source.labels.Count is 0) return;
        destination.labels.AddRange(source.labels);
        source.labels.Clear();
    }

    private static bool IsMethod(CodeInstruction instruction, string methodName, string declaringTypeName)
        => instruction.operand is MethodBase method && method.Name == methodName && method.DeclaringType?.Name == declaringTypeName;
}

///<summary>BaseLib cardmods get their own loop. Refund, Purge and the global hook remain outside it.</summary>
[HarmonyPatch]
public static class DeviousBaseLibModifierLoopPatch
{
    [HarmonyPrepare]
    public static bool Prepare() => TargetMethod() is not null;

    [HarmonyTargetMethod]
    public static MethodBase? TargetMethod()
    {
        var patchType = typeof(CardModifier).Assembly.GetType("BaseLib.Patches.Hooks.AfterCardPlayedPatch");
        var original = patchType?.GetMethod("BeforeAfterPlayHooks", AccessTools.all);
        if (original is null) return null;

        var asyncAttribute = original.GetCustomAttribute<AsyncStateMachineAttribute>();
        return asyncAttribute is null ? original : AccessTools.Method(asyncAttribute.StateMachineType, "MoveNext");
    }

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original, ILGenerator generator)
    {
        var list = instructions.ToList();
        var cardPlayField = original.DeclaringType?.GetFields(AccessTools.all).FirstOrDefault(f => f.FieldType == typeof(CardPlay) && f.Name.Contains("cardPlay"));
        if (cardPlayField is null) return list;

        var modifiersCall = list.FindIndex(i => IsMethod(i, "Modifiers", nameof(CardModifier)));
        var cardPlayLoad = FindPreviousFieldLoad(list, modifiersCall, cardPlayField);
        if (modifiersCall < 0 || cardPlayLoad < 2 || !IsModifierSetup(list, cardPlayLoad, modifiersCall)) return list;

        var enumeratorStore = FindEnumeratorStore(list, modifiersCall);
        if (enumeratorStore < 0 || list[enumeratorStore].operand is not FieldInfo enumeratorField) return list;

        var cleanup = FindEnumeratorCleanup(list, enumeratorStore, enumeratorField);
        var loopStartIndex = cardPlayLoad - 2;
        var loopEndIndex = cleanup + 1;
        if (cleanup < 0 || loopEndIndex >= list.Count) return list;

        var loopStartLabel = generator.DefineLabel();
        var modifierEndLabel = generator.DefineLabel();
        var beginIteration = AccessTools.Method(
            typeof(DeviousEffectLoopTracker), nameof(DeviousEffectLoopTracker.BeginModifierEffects));
        var shouldSkip = AccessTools.Method(
            typeof(DeviousEffectLoopTracker), nameof(DeviousEffectLoopTracker.ShouldSkipModifierEffects));
        var shouldRepeat = AccessTools.Method(
            typeof(DeviousEffectLoopTracker), nameof(DeviousEffectLoopTracker.ShouldRepeatModifierEffects));
        if (beginIteration is null || shouldSkip is null || shouldRepeat is null) return list;

        //Insert higher-offset first, and then a zero-effect branch, to clear the repeat helper scope before BaseLib's Refund/Purge bookkeeping.
        var loopback = new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, cardPlayField),
            new CodeInstruction(OpCodes.Call, shouldRepeat),
            new CodeInstruction(OpCodes.Brtrue, loopStartLabel),
        };
        MoveLabels(list[loopEndIndex], loopback[0]);
        loopback[0].labels.Add(modifierEndLabel);
        list.InsertRange(loopEndIndex, loopback);

        //Each modifier pass needs a fresh enumerator. The async state-machine field preserves it across await. 
        var begin = new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, cardPlayField),
            new CodeInstruction(OpCodes.Call, beginIteration),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, cardPlayField),
            new CodeInstruction(OpCodes.Call, shouldSkip),
            new CodeInstruction(OpCodes.Brtrue, modifierEndLabel),
        };
        MoveLabels(list[loopStartIndex], begin[0]);
        begin[0].labels.Add(loopStartLabel);
        list.InsertRange(loopStartIndex, begin);

        return list;
    }

    private static bool IsModifierSetup(IReadOnlyList<CodeInstruction> instructions, int cardPlayLoad, int modifiersCall)
        => instructions[cardPlayLoad - 2].opcode == OpCodes.Ldarg_0
           && instructions[cardPlayLoad - 1].opcode == OpCodes.Ldarg_0
           && IsMethod(instructions[cardPlayLoad + 1], "get_Card", nameof(CardPlay))
           && modifiersCall == cardPlayLoad + 2;

    private static int FindPreviousFieldLoad(IReadOnlyList<CodeInstruction> instructions, int before, FieldInfo field)
    {
        for (var i = before - 1; i >= 0; i--) if (instructions[i].opcode == OpCodes.Ldfld && Equals(instructions[i].operand, field)) return i;

        return -1;
    }

    private static int FindEnumeratorStore(IReadOnlyList<CodeInstruction> instructions, int after)
    {
        for (var i = after + 1; i < instructions.Count; i++)
        {
            if (instructions[i].opcode == OpCodes.Stfld && instructions[i].operand is FieldInfo { Name: "<>7__wrap1" }) return i;
        }

        return -1;
    }

    private static int FindEnumeratorCleanup(IReadOnlyList<CodeInstruction> instructions, int after, FieldInfo enumeratorField)
    {
        for (var i = after + 2; i < instructions.Count; i++)
        {
            if (instructions[i].opcode != OpCodes.Initobj
                || !Equals(instructions[i].operand, enumeratorField.FieldType)
                || instructions[i - 1].opcode != OpCodes.Ldflda
                || !Equals(instructions[i - 1].operand, enumeratorField)
                || instructions[i - 2].opcode != OpCodes.Ldarg_0)
            {
                continue;
            }

            return i;
        }

        return -1;
    }

    private static void MoveLabels(CodeInstruction source, CodeInstruction destination)
    {
        if (source.labels.Count is 0) return;
        destination.labels.AddRange(source.labels);
        source.labels.Clear();
    }

    private static bool IsMethod(CodeInstruction instruction, string methodName, string declaringTypeName)
        => instruction.operand is MethodBase method && method.Name == methodName && method.DeclaringType?.Name == declaringTypeName;
}

///<summary>Reuse matching first-pass Block results on later passes, avoiding recalculation after one-use modifiers are consumed.</summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyBlock))]
public static class DeviousBlockCapturePatch
{
    public static bool Prefix(CardPlay? cardPlay, CardModel? cardSource, Creature target, decimal block, ValueProp props, ref decimal __result, ref IEnumerable<AbstractModel> modifiers, out DeviousEffectLoopTracker.CaptureToken? __state)
    {
        __state = DeviousEffectLoopTracker.PrepareBlockCapture(cardPlay, cardSource, target, block, props, out var captured);
        if (captured is null) return true;

        __result = captured.Result;
        modifiers = captured.Modifiers;
        return false;
    }

    public static void Postfix(DeviousEffectLoopTracker.CaptureToken? __state, decimal __result, IEnumerable<AbstractModel> modifiers) => DeviousEffectLoopTracker.CompleteCapture(__state, __result, modifiers);
}

///<summary>Reuses first-iteration resolved damage at the same native calculation points, just like for Block.</summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyDamage))]
public static class DeviousDamageCapturePatch
{
    public static bool Prefix(
        CardPlay? cardPlay,
        CardModel? cardSource,
        Creature? target,
        Creature? dealer,
        decimal damage,
        ValueProp props,
        ModifyDamageHookType modifyDamageHookType,
        CardPreviewMode previewMode,
        ref decimal __result,
        ref IEnumerable<AbstractModel> modifiers,
        out DeviousEffectLoopTracker.CaptureToken? __state)
    {
        __state = DeviousEffectLoopTracker.PrepareDamageCapture(
            cardPlay, cardSource, target, dealer, damage, props,
            modifyDamageHookType, previewMode, out var captured);
        if (captured is null) return true;

        __result = captured.Result;
        modifiers = captured.Modifiers;
        return false;
    }

    public static void Postfix(
        DeviousEffectLoopTracker.CaptureToken? __state,
        decimal __result,
        IEnumerable<AbstractModel> modifiers)
        => DeviousEffectLoopTracker.CompleteCapture(__state, __result, modifiers);
}

/// <summary>Temporarily report a consumed enchantment as active during the loop; the underlying status stays disabled afterward.</summary>
[HarmonyPatch(typeof(EnchantmentModel), "get_Status")]
public static class DeviousEnchantmentStatusPatch
{
    public static void Postfix(EnchantmentModel __instance, ref EnchantmentStatus __result)
    {
        if (DeviousEffectLoopTracker.ShouldShowEnchantmentAsNormal(__instance, __result))
            __result = EnchantmentStatus.Normal;
    }
}

///<summary>Tainted is jank, effect is called in Vital Spark instead of Affliction.</summary>
[HarmonyPatch(typeof(AfflictionModel), nameof(AfflictionModel.OnPlay))]
public static class DeviousTaintedEffectPatch
{
    private static readonly MethodInfo FlashMethod = AccessTools.Method(typeof(PowerModel), "Flash")
        ?? throw new MissingMethodException(typeof(PowerModel).FullName, "Flash");

    public static void Postfix(AfflictionModel __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        if (__instance is not Tainted tainted || !DeviousEffectLoopTracker.ShouldRunTaintedEffects(tainted))
            return;

        __result = ApplyVitalSparkEffects(__result, tainted, choiceContext);
    }

    private static async Task ApplyVitalSparkEffects(
        Task nativeEffect,
        Tainted tainted,
        PlayerChoiceContext choiceContext)
    {
        await nativeEffect;
        var vitalSparks = tainted.CombatState.IterateHookListeners().OfType<VitalSparkPower>().ToArray();
        foreach (var power in vitalSparks)
        {
            //Match the model scope which Hook.AfterCardPlayed would normally provide.
            choiceContext.PushModel(power);
            try
            {
                FlashMethod.Invoke(power, null);
                await PowerCmd.Apply<TaintedPower>(
                    choiceContext,
                    tainted.Card.Owner.Creature,
                    power.Amount,
                    null,
                    null);
            }
            finally
            {
                choiceContext.PopModel(power);
            }
        }
    }
}

///<summary>Prevent Vital Spark from applying Tainted an additional time after Devious loops have finished.</summary>
[HarmonyPatch(typeof(VitalSparkPower), nameof(VitalSparkPower.AfterCardPlayed))]
public static class DeviousTaintedAfterCardPlayedPatch
{
    public static bool Prefix(CardPlay cardPlay, ref Task __result)
    {
        if (!DeviousEffectLoopTracker.ShouldSuppressNativeTaintedAfterCardPlayed(cardPlay))
            return true;

        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch]
public static class DeviousEffectLoopCombatStateCleanupPatch
{
    [HarmonyTargetMethod]
    public static MethodBase? TargetMethod()
        => typeof(CombatState).GetConstructors().FirstOrDefault();

    [HarmonyPostfix]
    public static void Clear() => DeviousEffectLoopTracker.ClearAll();
}