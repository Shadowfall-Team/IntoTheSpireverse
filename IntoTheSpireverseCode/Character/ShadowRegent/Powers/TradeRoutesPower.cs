using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;

public class TradeRoutesPower : ShadowPowerModel, IDrawFromPileModifier
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public Task AfterModifyingStartOfTurnDrawFromPile(PileType pileType)
    {
        Flash();
        return Task.CompletedTask;
    }

    public decimal ModifyStartOfTurnDrawFromPile(PileType pileType, Player player, decimal originalCardCount)
    {
        if (player == Owner.Player && pileType == CargoCardPile.CargoPileType)
        {
            return originalCardCount += Amount;
        }
        return originalCardCount;
    }
}