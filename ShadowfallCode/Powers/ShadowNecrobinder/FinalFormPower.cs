using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Shadowfall.ShadowfallCode.Powers.ShadowNecrobinder;

public class FinalFormPower : CustomPowerModel
{
    private const string _cardKey = "Card";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;

    protected override object InitInternalData() => new Data();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("Card"),
    ];

    public override async Task AfterPlayerTurnStartEarly(
        PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        var card = GetInternalData<Data>().selectedCard;
        if (card == null) return;

        Flash();
        await CardCmd.AutoPlay(choiceContext, card.CreateDupe(), null);
        await PowerCmd.Decrement(this);
    }

    public void SetSelectedCard(CardModel card)
    {
        GetInternalData<Data>().selectedCard = card.CreateClone();
        ((StringVar)DynamicVars["Card"]).StringValue = card.Title;
    }

    private class Data
    {
        public CardModel? selectedCard;
    }
}