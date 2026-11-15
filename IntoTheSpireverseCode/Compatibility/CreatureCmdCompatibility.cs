using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class CreatureCmdCompatibility
{
    // LoseMaxHp compat (111 / 113)
    private delegate Task v111Struct(PlayerChoiceContext choiceContext, Creature creature, decimal amount, bool isFromCard);
    private delegate Task v113Struct(PlayerChoiceContext choiceContext, Creature creature, decimal amount, CardModel? cardSource);
    private static readonly v111Struct? v111LoseMaxHp;
    private static readonly v113Struct? v113LoseMaxHp;

    static CreatureCmdCompatibility()
    {
        v111LoseMaxHp = AccessTools.MethodDelegate<v111Struct>(AccessTools.Method(typeof(CreatureCmd), nameof(CreatureCmd.LoseMaxHp),
            parameters: [typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal), typeof(bool)]));

        v113LoseMaxHp = AccessTools.MethodDelegate<v113Struct>(AccessTools.Method(typeof(CreatureCmd), nameof(CreatureCmd.LoseMaxHp),
            parameters: [typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal), typeof(CardModel)]));

        if (v111LoseMaxHp is null && v113LoseMaxHp is null)
        {
            throw new MethodAccessException("No recognised signature found for CreatureCmd.LoseMaxHp");
        }
    }

    public static Task LoseMaxHp(PlayerChoiceContext choiceContext, Creature creature, decimal amount, CardModel? cardSource)
    {
        if (v111LoseMaxHp is not null)
        {
            return v111LoseMaxHp(choiceContext, creature, amount, cardSource is not null);
        }
        else if (v113LoseMaxHp is not null)
        {
            return v113LoseMaxHp(choiceContext, creature, amount, cardSource);
        }
        else { throw new MethodAccessException("No recognised signature found for CreatureCmd.LoseMaxHp"); }
    }


    // Lamali's compat for Damage and LoseBlock
    // --- public API, identical on both game versions ---

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        => SingleWithDealer(choiceContext, target, amount, props, dealer, cardSource, cardPlay);

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, CardModel cardSource, CardPlay? cardPlay)
        => SingleCardOnly(choiceContext, target, amount, props, cardSource, cardPlay);

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        => MultiWithDealer(choiceContext, targets, amount, props, dealer, cardSource, cardPlay);

    // --- plumbing ---

    private delegate Task<IEnumerable<DamageResult>> SingleDealerDel(
        PlayerChoiceContext ctx, Creature target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? card, CardPlay? play);

    private delegate Task<IEnumerable<DamageResult>> SingleCardDel(
        PlayerChoiceContext ctx, Creature target, decimal amount,
        ValueProp props, CardModel card, CardPlay? play);

    private delegate Task<IEnumerable<DamageResult>> MultiDealerDel(
        PlayerChoiceContext ctx, IEnumerable<Creature> targets, decimal amount,
        ValueProp props, Creature? dealer, CardModel? card, CardPlay? play);

    private static readonly SingleDealerDel SingleWithDealer = Build<SingleDealerDel>(
        typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
        typeof(ValueProp), typeof(Creature), typeof(CardModel));

    private static readonly SingleCardDel SingleCardOnly = Build<SingleCardDel>(
        typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
        typeof(ValueProp), typeof(CardModel));

    private static readonly MultiDealerDel MultiWithDealer = Build<MultiDealerDel>(
        typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal),
        typeof(ValueProp), typeof(Creature), typeof(CardModel));

    /// <summary>
    /// Finds CreatureCmd.Damage matching baseParams (optionally + CardPlay) and
    /// compiles a delegate whose signature always includes the CardPlay parameter.
    /// On old versions the CardPlay argument is simply dropped.
    /// </summary>
    private static TDelegate Build<TDelegate>(params Type[] baseParams) where TDelegate : Delegate
    {
        var withPlay = baseParams.Append(typeof(CardPlay)).ToArray();

        var method = typeof(CreatureCmd).GetMethod("Damage",
                         BindingFlags.Public | BindingFlags.Static, null, withPlay, null)
                     ?? FindWithTrailingOptionals(withPlay)
                     ?? typeof(CreatureCmd).GetMethod("Damage",
                         BindingFlags.Public | BindingFlags.Static, null, baseParams, null)
                     ?? throw new MissingMethodException(
                         $"CreatureCmd.Damage({string.Join(", ", baseParams.Select(t => t.Name))}) not found");

        var methodParams = method.GetParameters();
        var hasPlay = methodParams.Length >= withPlay.Length && methodParams[withPlay.Length - 1].ParameterType == typeof(CardPlay);

        // Lambda parameters always follow the *new* signature (incl. CardPlay).
        var lambdaParams = withPlay.Select((t, i) => Expression.Parameter(t, $"p{i}")).ToArray();
        var callArgs = (hasPlay ? lambdaParams : lambdaParams[..^1]).Cast<Expression>().ToList();
        // Newer builds append optional parameters (e.g. ModifyDamageHookType); fill them with their defaults.
        callArgs.AddRange(methodParams.Skip(callArgs.Count)
            .Select(p => Expression.Constant(p.DefaultValue, p.ParameterType)));

        var call = Expression.Call(method, callArgs);
        return Expression.Lambda<TDelegate>(call, lambdaParams).Compile();
    }

    /// <summary>A `Damage` overload whose leading parameters are exactly <paramref name="leading"/>, followed only by optional ones.</summary>
    private static MethodInfo? FindWithTrailingOptionals(Type[] leading)
    {
        return typeof(CreatureCmd).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
            {
                if (m.Name != "Damage") return false;
                var ps = m.GetParameters();
                return ps.Length > leading.Length
                       && ps.Take(leading.Length).Select(p => p.ParameterType).SequenceEqual(leading)
                       && ps.Skip(leading.Length).All(p => p.IsOptional);
            });
    }

    // For Hijack compatibility
    private static readonly LoseBlockDel LoseBlockImpl = BuildLoseBlock();
    
    public static Task LoseBlock(PlayerChoiceContext choiceContext, Creature target, Decimal amount, Creature? remover)
    {
        return LoseBlockImpl(choiceContext, target, amount, remover);
    }
    
    private static LoseBlockDel BuildLoseBlock()
    {
        // New: LoseBlock(PlayerChoiceContext, Creature, decimal, Creature?)
        var newMethod = typeof(CreatureCmd).GetMethod("LoseBlock",
            BindingFlags.Public | BindingFlags.Static, null,
            [typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal), typeof(Creature)], null);

        // Old: LoseBlock(Creature, int)
        var oldMethod = typeof(CreatureCmd).GetMethod("LoseBlock",
            BindingFlags.Public | BindingFlags.Static, null,
            [typeof(Creature), typeof(decimal)], null);

        var ctx = Expression.Parameter(typeof(PlayerChoiceContext), "ctx");
        var target = Expression.Parameter(typeof(Creature), "target");
        var amount = Expression.Parameter(typeof(decimal), "amount");
        var remover = Expression.Parameter(typeof(Creature), "remover");

        Expression call;
        if (newMethod != null)
            call = Expression.Call(newMethod, ctx, target, amount, remover);
        else if (oldMethod != null)
            // ctx and remover dropped; decimal truncated to int like a C# explicit cast
            call = Expression.Call(oldMethod, target, amount);
        else
            throw new MissingMethodException("CreatureCmd.LoseBlock not found in either known signature");

        return Expression.Lambda<LoseBlockDel>(call, ctx, target, amount, remover).Compile();
    }
    
    private delegate Task LoseBlockDel(
        PlayerChoiceContext ctx, Creature target, decimal amount, Creature? remover);
}