using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;

public class AmmoCardPile() : CustomPile(AmmoPileType)
{
    [CustomEnum] public static PileType AmmoPileType;

    public override bool CardShouldBeVisible(CardModel card)
    {
        return true;
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        return new Vector2(1820, 900);
    }
}
