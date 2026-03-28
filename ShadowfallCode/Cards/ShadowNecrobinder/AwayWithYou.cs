using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class AwayWithYou() : ShadowNecrobinderCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool CanBeGeneratedInCombat => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);
        var card = selected.FirstOrDefault();
        if (card == null) return;

        await CardCmd.Exhaust(choiceContext, card);

        if (card.DeckVersion != null && card.DeckVersion.Pile?.Type == PileType.Deck)
        {
            await CardPileCmd.RemoveFromDeck(card.DeckVersion);
            await CreatureCmd.LoseMaxHp(choiceContext, Owner.Creature, 5m, true);
        }
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}