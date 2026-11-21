using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class PureSuffering() : ShadowSilentCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const string _deviousKey = "Devious";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new (_deviousKey, 0)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(IsUpgraded ? IntoTheSpireverseKeywords.DeviousX : IntoTheSpireverseKeywords.Devious)];

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

        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, VfxCmd.slashPath);
        attackContext.AddHit(await CreatureCmdCompatibility.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, Owner.Creature, this, cardPlay));

        await Cmd.CustomScaledWait(0.1f, 0.25f);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CreateClone(), PileType.Discard, Owner), 2.2f);

        if (DeviousEffectLoopTracker.ShouldAddThumbscrewDamageHit(cardPlay))
        {
            VfxCmd.PlayOnCreatureCenter(cardPlay.Target, VfxCmd.slashPath);
            attackContext.AddHit(await CreatureCmdCompatibility.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, Owner.Creature, this, cardPlay));
        }
    }

    protected override void OnUpgrade() => DynamicVars[_deviousKey].UpgradeValueBy(1);
}