using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;

public class CargoCardPile() : CustomPile(CargoPileType)
{
    [CustomEnum] public static PileType CargoPileType;

    public override bool CardShouldBeVisible(CardModel card)
    {
        return true;
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        return new Vector2(75, 765); // Cargo pile position
    }
}

public struct CargoSelectorPrefs
{
    public static LocString ToCargoSelectionPrompt => new LocString("card_selection", "INTOTHESPIREVERSE-TO_CARGO");
    public static LocString FromCargoSelectionPrompt => new LocString("card_selection", "INTOTHESPIREVERSE-FROM_CARGO");
}
