using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowIronclad;

public sealed class VampiricStrike() : ShadowIroncladCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const string HpThresholdKey = "HpThreshold";

    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8m, ValueProp.Move),
        new HealVar(3m),
        new(HpThresholdKey, 50m)
    ];

    protected override bool ShouldGlowGoldInternal => IsAtOrBelowHalfHp;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (IsAtOrBelowHalfHp)
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Heal.UpgradeValueBy(1m);
    }

    private bool IsAtOrBelowHalfHp => Owner.Creature.CurrentHp <= Owner.Creature.MaxHp / 2;
}