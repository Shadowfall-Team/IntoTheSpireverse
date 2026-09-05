using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

/// <summary>
/// Slate is spent as it is used, so the discount has to track the current amount rather than
/// accumulate like Stomp's or Midnight's. Only the delta since the last sync is applied, which keeps
/// the net local modifier equal to the Slate on the board at any moment.
/// </summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Hammer() : ShadowIroncladCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private int _appliedReduction;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(22m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SlatePower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithAttackerAnim(Ironclad.GetHeavyAnimIfApplicable(Owner.Character),
                Ironclad.GetHeavyAttackDelayIfApplicable(Owner.Character))
            .WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3")
            .WithHitVfxSpawnedAtBase()
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card == this) SyncCostToSlate();
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is SlatePower) SyncCostToSlate();
        return Task.CompletedTask;
    }

    private void SyncCostToSlate()
    {
        if (IsCanonical) return;

        var slate = Owner?.Creature.Powers.OfType<SlatePower>().FirstOrDefault()?.Amount ?? 0;
        var delta = slate - _appliedReduction;
        if (delta == 0) return;

        EnergyCost.AddThisCombat(-delta);
        _appliedReduction = slate;
    }
}
