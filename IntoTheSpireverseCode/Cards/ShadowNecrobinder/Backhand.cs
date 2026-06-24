using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

public sealed class Backhand() : ShadowNecrobinderCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    // Draw a Curse
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner || card.Type != CardType.Curse) return;
        await TryMoveToHand();
    }

    // Exhaust a Curse
    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPile, AbstractModel? source)
    {
        if (card == this || card.Owner != Owner || card.Type != CardType.Curse) return;
        if (card.Pile?.Type == PileType.Exhaust)
        {
            await TryMoveToHand();
        }
    }

    // Create a Curse
    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card == this || card.Owner != Owner || card.Type != CardType.Curse) return;
        await TryMoveToHand();
    }

    private async Task TryMoveToHand()
    {
        if (Pile?.Type == PileType.Deck) return; // don't add the deck copy to the hand?
        if (Pile?.Type == PileType.Hand) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }
}
