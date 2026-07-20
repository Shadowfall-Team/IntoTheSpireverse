using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class RetainCardPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        RetainCardPower source = this;
        if (player != source.Owner.Player || !Hook.ShouldFlush(player.Creature.CombatState, player))
            return;
        CardSelectorPrefs prefs = new CardSelectorPrefs(source.SelectionScreenPrompt, 0, source.Amount);
        List<CardModel> list = (await CardSelectCmd.FromHand(choiceContext, source.Owner.Player, prefs, new Func<CardModel, bool>(source.RetainFilter), (AbstractModel) source)).ToList<CardModel>();
        if (list.Count == 0)
            return;
        foreach (CardModel cardModel in list)
            cardModel.GiveSingleTurnRetain();
    }
    
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        RetainCardPower power = this;
        if (!participants.Contains(power.Owner))
            return;
        await PowerCmd.Remove(power);
    }

    private bool RetainFilter(CardModel card) => !card.ShouldRetainThisTurn;
}