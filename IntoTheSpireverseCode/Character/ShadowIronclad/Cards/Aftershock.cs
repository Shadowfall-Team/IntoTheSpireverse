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

        // X-1 unupgraded, X upgraded. An X of 0 leaves the threshold at -1, below the cheapest card
        // the player can name, so a 0-energy Aftershock offers nothing at all.
        var xCost = ResolveEnergyXValue() - (IsUpgraded ? 0 : 1);
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var card = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            [.. PileType.Discard.GetPile(Owner).Cards.Where(c => SelectableCost(c) <= xCost)],
            Owner,
            prefs)).FirstOrDefault();

        if (card == null) return;

        // Replay rather than two AutoPlay calls. A Power leaves the Play pile for the player's power
        // list as soon as it resolves, and an Exhaust card goes to the Exhaust pile, so a second
        // AutoPlay on the same card finds it somewhere it cannot be played from and silently does
        // nothing. Replay resolves the card repeatedly inside one play, which is how Echo Form and
        // the Replay keyword get the same wording to work.
        //
        // BaseReplayCount is normally a permanent property, so it is restored afterwards to avoid
        // leaving the card replaying itself for the rest of the run.
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

    /// <summary>
    /// The cost to compare against the threshold, as the player understands it.
    ///
    /// An X-cost card is worth 0 here: the least X can be is 0, so that is the cheapest the card can
    /// ever be played for. CardEnergyCost.GetResolved is the wrong question to ask one of these,
    /// because for an X-cost card it returns CapturedXValue, which describes whatever X was the last
    /// time the card happened to be played rather than anything about the card sitting in the pile.
    /// </summary>
    private static int SelectableCost(CardModel card) =>
        card.EnergyCost.CostsX ? 0 : card.EnergyCost.GetResolved();
}
