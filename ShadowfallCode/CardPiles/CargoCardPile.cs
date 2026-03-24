using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Shadowfall.ShadowfallCode.CardPiles;

public class CargoCardPile() : CustomPile(CargoPileType)
{
    
    [CustomEnum]
    public static PileType CargoPileType;
    
    public override bool CardShouldBeVisible(CardModel card)
    {
        throw new NotImplementedException();
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        throw new NotImplementedException();
    }
}