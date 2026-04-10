using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Shadowfall.ShadowfallCode.CardPiles;

public class CargoCardPile() : CustomPile(CargoPileType)
{
    [CustomEnum] public static PileType CargoPileType;

    //TODO: make this visible
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
    //TODO: add localizations
    public static LocString ToCargoSelectionPrompt => new LocString("card_selection", "TO_CARGO");
    public static LocString FromCargoSelectionPrompt => new LocString("card_selection", "FROM_CARGO");
}