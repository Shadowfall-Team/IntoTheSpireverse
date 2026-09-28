using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Aftershock() : ShadowIroncladCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int Plays = 2;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatManager.Instance.IsOverOrEnding) return;

        // X-1 unupgraded, X upgraded.
        var xCost = ResolveEnergyXValue() - (IsUpgraded ? 0 : 1);
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var card = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            [.. PileType.Discard.GetPile(Owner).Cards.Where(c => SelectableCost(c) <= xCost)],
            Owner,
            prefs)).FirstOrDefault();

        if (card == null) return;

        // Replay, not a second AutoPlay: once resolved, a Power or Exhaust card is no longer
        // somewhere it can be played from. BaseReplayCount persists, so it is restored after.
        var replayCountBefore = card.BaseReplayCount;
        try
        {
            card.BaseReplayCount = replayCountBefore + Plays - 1;
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
        finally
        {
            card.BaseReplayCount = replayCountBefore;
        }
    }

    // X-cost cards count as 0. GetResolved would return the X captured on their last play.
    private static int SelectableCost(CardModel card) =>
        card.EnergyCost.CostsX ? 0 : card.EnergyCost.GetResolved();
}
