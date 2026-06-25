using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

public sealed class Smush() : ShadowNecrobinderCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new EnergyVar(2),
    ];

    protected override bool ShouldGlowGoldInternal => HasPlayedExpensiveThisTurn;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || !HasPlayedExpensiveThisTurn) return Task.CompletedTask;
        ReduceCost();
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner) return Task.CompletedTask;
        if (cardPlay.Card.EnergyCost.GetResolved() < DynamicVars.Energy.IntValue) return Task.CompletedTask;
        ReduceCost();
        return Task.CompletedTask;
    }

    private void ReduceCost() => EnergyCost.SetThisTurn(0);

    private bool HasPlayedExpensiveThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished
            .Any(e => e.CardPlay.Card.Owner == Owner
                      && e.HappenedThisTurn(CombatState)
                      && e.CardPlay.Card.EnergyCost.GetResolved() >= DynamicVars.Energy.IntValue);
}