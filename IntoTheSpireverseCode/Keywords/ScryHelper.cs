using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

/// <summary>
/// Scry is not a base-game StS2 mechanic - it does not exist anywhere in v0.111.0 - so it is
/// implemented here as a Spireverse keyword: look at the top N cards of your Draw Pile and discard
/// any number of them.
/// </summary>
public static class ScryHelper
{
    private static LocString Prompt => new("card_selection", "INTOTHESPIREVERSE-SCRY_PROMPT");

    public static async Task Scry(PlayerChoiceContext choiceContext, Player player, int amount)
    {
        if (amount <= 0) return;

        // Cards[0] is the top of the Draw Pile (see GalacticStrike, AncestralEcho).
        var top = PileType.Draw.GetPile(player).Cards.Take(amount).ToList();
        if (top.Count == 0) return;

        var prefs = new CardSelectorPrefs(Prompt, 0, top.Count);
        var chosen = (await CardSelectCmd.FromSimpleGrid(choiceContext, top, player, prefs)).ToList();
        if (chosen.Count == 0) return;

        await CardCmd.Discard(choiceContext, chosen);
    }
}
