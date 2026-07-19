
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;

public class RecallPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation location)
    {
        if (card.Owner.Creature != Owner)
            return location;
        if (card.IsDupe)
            return location;
        if (location.pileType == PileType.None)
            return location;
        location.pileType = PileType.Hand;
        location.position = CardPilePosition.Top;
        return location;
    }

    public override Task AfterModifyingCardPlayResultLocation(
        CardModel card, CardLocation location)
    {
        Flash();
        PowerCmd.Decrement(this);
        return Task.CompletedTask;
    }
}