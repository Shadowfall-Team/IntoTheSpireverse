using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class PureSuffering() : ShadowSilentCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const string _deviousKey = "Devious";
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new DynamicVar(_deviousKey, 0m),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
    ];
    
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IsUpgraded ? HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.DeviousX) : HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        if (CombatState == null) return;

        await using AttackContext attackContext = await AttackContextCompatibility.CreateContextAsync(CombatState, choiceContext, cardPlay);
        await IntoTheSpireverseKeywords.ExecuteDevious(choiceContext, Owner, this, DynamicVars[_deviousKey].IntValue, async () =>
        {
            VfxCmd.PlayOnCreatureCenter(cardPlay.Target, VfxCmd.slashPath);
            attackContext.AddHit(await CreatureCmdCompatibility.Damage(
                choiceContext,
                cardPlay.Target,
                DynamicVars.Damage.BaseValue,
                DynamicVars.Damage.Props,
                Owner.Creature,
                this,
                cardPlay
            ));
            await Cmd.CustomScaledWait(0.1f, 0.25f);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CreateClone(), PileType.Discard, Owner), 2.2f);
        });
    }

    protected override void OnUpgrade()
    {
        DynamicVars[_deviousKey].UpgradeValueBy(1m);
    }
}
