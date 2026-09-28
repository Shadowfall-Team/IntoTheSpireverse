using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class DigDeep() : ShadowIroncladCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self),
    ICardDestinationListener
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(2m),
        new CardsVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
    ];

    // Playable any number of times per turn now; the second and later plays Exhaust instead of
    // discarding. The red glow signals that the card is on its follow-up play.
    protected override bool ShouldGlowRedInternal => HasBeenPlayedThisTurn;

    public CardDestination ModifyCardDestination(
        CardModel card, bool isAutoPlay, ResourceInfo resources, CardDestination destination) =>
        card == this && HasBeenPlayedThisTurn
            ? destination with { PileType = PileType.Exhaust }
            : destination;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmdCompatibility.Damage(choiceContext, Owner.Creature,
            DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, this, cardPlay);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);

    private bool HasBeenPlayedThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished
            .Any(e => e.CardPlay.Card == this && e.HappenedThisTurn(CombatState));
}
