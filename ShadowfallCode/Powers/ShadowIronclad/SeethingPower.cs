using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class SeethingPower : CustomPowerModel
{
    private const string _cardKey = "Card";

    public override PowerType Type => PowerType.Buff;
    public override bool IsInstanced => true;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData() => new Data();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar(_cardKey),
    ];

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player != Owner.Player) return;

        var card = GetInternalData<Data>().selectedCard;
        if (card == null) return;
        Flash();
        for (int i = 0; i < Amount; i++)
        {
            await CardPileCmd.AddGeneratedCardToCombat(card.CreateClone(), PileType.Hand, true);
        }
        await PowerCmd.Remove(this);
    }

    public void SetSelectedCard(CardModel card)
    {
        GetInternalData<Data>().selectedCard = card.CreateClone();
        ((StringVar)DynamicVars[_cardKey]).StringValue = card.Title;
    }

    private class Data
    {
        public CardModel? selectedCard;
    }
}