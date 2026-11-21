using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class Haymaker() : ShadowSilentCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    private const string _deviousKey = "Devious";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14, ValueProp.Move), new PowerVar<WeakPower>(3), new (_deviousKey, 0)];
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromKeyword(IsUpgraded ? IntoTheSpireverseKeywords.DeviousX : IntoTheSpireverseKeywords.Devious)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        if (CombatState is null) return;

        await using var attackContext = await AttackContextCompatibility.CreateContextAsync(CombatState, choiceContext, cardPlay);
        var state = DeviousEffectLoopTracker.GetState(cardPlay);
        if (!state.Initialized)
        {
            state.RepeatCount = Math.Max(0, await IntoTheSpireverseKeywords.ModifyDeviousRepeatCount(choiceContext, Owner, this, DynamicVars[_deviousKey].IntValue));
            state.Initialized = true;
        }

        if (state.Iteration >= state.RepeatCount) return;

        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, VfxCmd.bluntPath);
        attackContext.AddHit(await CreatureCmdCompatibility.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, Owner.Creature, this, cardPlay));

        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars.Weak.BaseValue, Owner.Creature, this);
        await Cmd.Wait(0.5f);

        if (DeviousEffectLoopTracker.ShouldAddThumbscrewDamageHit(cardPlay))
        {
            VfxCmd.PlayOnCreatureCenter(cardPlay.Target, VfxCmd.bluntPath);
            attackContext.AddHit(await CreatureCmdCompatibility.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, Owner.Creature, this, cardPlay));
        }
    }

    protected override void OnUpgrade() => DynamicVars[_deviousKey].UpgradeValueBy(1);
}