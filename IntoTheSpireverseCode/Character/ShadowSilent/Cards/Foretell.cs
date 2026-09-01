using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class Foretell() : ShadowSilentCard(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    private const string _deviousKey = "Devious";
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3m, ValueProp.Move),
        new DynamicVar(_deviousKey, 0m),
    ];
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IsUpgraded ? HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.DeviousX) : HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await IntoTheSpireverseKeywords.ExecuteDevious(choiceContext, Owner, this, DynamicVars[_deviousKey].IntValue, () =>
            CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay));
	}

    protected override void OnUpgrade()
    {
        DynamicVars[_deviousKey].UpgradeValueBy(1m);
    }
}
