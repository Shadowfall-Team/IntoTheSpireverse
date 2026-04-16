using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class WarcraftPower : CustomPowerModel
{
    private int _cardsPlayedThisTurn;
    private const int Threshold = 5;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => Threshold - _cardsPlayedThisTurn;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!CombatManager.Instance.IsInProgress || cardPlay.IsAutoPlay || cardPlay.Card.Owner.Creature != Owner)
            return;
        if (_cardsPlayedThisTurn >= Threshold)
            return;

        _cardsPlayedThisTurn++;
        InvokeDisplayAmountChanged();

        if (_cardsPlayedThisTurn != Threshold)
            return;

        Flash();

        var pool = Owner.Player.Character.CardPool
            .GetUnlockedCards(Owner.Player.UnlockState, Owner.Player.RunState.CardMultiplayerConstraint)
            .Where(c => c.Type == CardType.Attack)
            .ToList();

        if (pool.Count == 0)
            return;

        var cards = CardFactory.GetDistinctForCombat(Owner.Player, pool, Amount, Owner.Player.RunState.Rng.CombatCardGeneration).ToList();
        foreach (var card in cards)
            card.SetToFreeThisTurn();

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, true);
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        CombatState combatState)
    {
        if (side != Owner.Side)
            return Task.CompletedTask;

        _cardsPlayedThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}