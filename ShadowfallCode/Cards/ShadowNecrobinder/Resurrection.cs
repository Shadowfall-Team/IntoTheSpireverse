using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class Resurrection() : ShadowNecrobinderCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (Creature creature in CombatState.PlayerCreatures.Where(c => c != null && c.IsAlive).ToList())
        {
            var discard = PileType.Discard.GetPile(creature.Player).Cards;
            if (discard.Count == 0) continue;

            CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
            CardModel card = (await CardSelectCmd.FromSimpleGrid(choiceContext, discard, creature.Player, prefs)).FirstOrDefault();
            if (card == null) continue;

            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}