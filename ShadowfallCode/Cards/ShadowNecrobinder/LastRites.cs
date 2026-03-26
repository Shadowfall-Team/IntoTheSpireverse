using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class LastRites() : ShadowNecrobinderCard(6, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private int _appliedReduction;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(25m, ValueProp.Move),
        new EnergyVar(1),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card == this) RecalculateCost();
        return Task.CompletedTask;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPile, AbstractModel? source)
    {
        if (card != this) RecalculateCost();
        return Task.CompletedTask;
    }

    private void RecalculateCost()
    {
        int curseCount = Owner.PlayerCombatState.AllCards.Count(c => c.Type == CardType.Curse);
        int newReduction = curseCount * DynamicVars.Energy.IntValue;
        int delta = newReduction - _appliedReduction;
        if (delta != 0)
        {
            EnergyCost.AddThisCombat(-delta);
            _appliedReduction = newReduction;
        }
    }
}