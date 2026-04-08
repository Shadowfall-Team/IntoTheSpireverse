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
        return false;
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        return new Vector2(250, 250);
    }
}

public struct CargoSelectorPrefs
{
    //TODO: add localizations
    public static LocString ToCargoSelectionPrompt => new LocString("card_selection", "TO_CARGO");
    public static LocString FromCargoSelectionPrompt => new LocString("card_selection", "FROM_CARGO");
}