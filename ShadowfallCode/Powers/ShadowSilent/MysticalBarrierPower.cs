using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public sealed class MysticalBarrierPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData() => new Data();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Ward>(false),
        HoverTipFactory.FromPower<WeakPower>(),
    ];

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            foreach (var card in pile.GetPile(Owner.Player).Cards)
            {
                if (card is Ward)
                    CardCmd.ApplyKeyword(card, CardKeyword.Retain);
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card.Owner.Creature != Owner)
            return Task.CompletedTask;
        if (card is not Ward)
            return Task.CompletedTask;

        CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;
        if (cardPlay.Card is not Ward)
            return;

        var data = GetInternalData<Data>();
        if (data.wardPlayedThisTurn)
            return;

        data.wardPlayedThisTurn = true;
        Flash();

        var target = Owner.Player.RunState.Rng.CombatTargets
            .NextItem(CombatState.HittableEnemies);

        if (target != null)
        {
            await PowerCmd.Apply<WeakPower>(
                choiceContext, target,
                (decimal)Amount, Owner, null);
        }
    }

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;

        GetInternalData<Data>().wardPlayedThisTurn = false;
        return Task.CompletedTask;
    }

    private class Data
    {
        public bool wardPlayedThisTurn;
    }
}