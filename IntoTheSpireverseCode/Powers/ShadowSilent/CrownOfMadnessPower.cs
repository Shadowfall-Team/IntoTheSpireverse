
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
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
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;

public class CrownOfMadnessPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override PowerStackType StackType => PowerStackType.None;
    
    
    protected override object InitInternalData() => new Data();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("Card")
    ];
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player == Owner.Player)
        {
            CardModel card = GetInternalData<Data>().selectedCard;
            if (!card.IsInCombat)
            {
                await PowerCmd.Remove(this);
                return;
            }
            await CardPileCmd.Add(card, PileType.Hand);
            
        }
    }

    public void SetSelectedCard(CardModel card)
    {
        GetInternalData<Data>().selectedCard = card;
        ((StringVar) DynamicVars["Card"]).StringValue = card.Title;
    }
    
    private class Data
    {
        public CardModel? selectedCard;
    }
}