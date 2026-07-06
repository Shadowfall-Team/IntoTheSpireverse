
using Godot;
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
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;

public class LuckyStreakPower : ShadowPowerModel, IntoTheSpireverseKeywords.ICardMuddledListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    

    protected override object InitInternalData() => new Data();
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle),
        HoverTipFactory.ForEnergy(this),
    ];
    
    public async Task AfterCardMuddled(ICombatState combatState, CardModel cardModel)
    {
        if (cardModel.Owner != Owner.Player) 
            return;
        var data = GetInternalData<Data>();
        if (data.cardMuddledThisTurn)
            return;
        Flash();
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        data.cardMuddledThisTurn = true;
        for (int i = 0; i < Amount; ++i)
            await PlayerCmd.GainEnergy(1M, Owner.Player);
    }
    
    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;

        GetInternalData<Data>().cardMuddledThisTurn = false;
        return Task.CompletedTask;
    }
    
    private class Data
    {
        public bool cardMuddledThisTurn;
    }
}