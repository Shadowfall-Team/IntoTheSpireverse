using BaseLib.Abstracts;
using BaseLib.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Ammo;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public class AmmoVolley() : CustomCardModel(1,
    CardType.Attack,
    CardRarity.Token,
    TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(14),
        new ExtraDamageVar(1),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(static (card, _) =>
                card.Owner.Creature.GetPowerAmount<NextVolleyDamagePower>() +
                card.Owner.Creature.GetPowerAmount<VolleyDamagePower>()),
        new RepeatVar(0),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [BaseLibKeywords.Purge];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var baseDamage = DynamicVars.CalculationBase.BaseValue;
        var extraDamage = DynamicVars.ExtraDamage.BaseValue;
        var multiplier = Owner.Creature.GetPowerAmount<NextVolleyDamagePower>()
                         + Owner.Creature.GetPowerAmount<VolleyDamagePower>();
        var damage = baseDamage + extraDamage * multiplier;

        var command = DamageCmd.Attack(damage)
            .WithHitCount(1)
            .FromCard(this)
            .WithAttackerAnim("Cast", Owner.Character.AttackAnimDelay)
            .WithAttackerFx(null, "event:/sfx/characters/regent/regent_sovereign_blade", null);

        // Use TargetingAllOpponents when BigGunsPower is active
        if (Owner.Creature.HasPower<BigGunsPower>())
        {
            command.TargetingAllOpponents(Owner.Creature.CombatState);
        }
        else
        {
            command.TargetingRandomOpponents(Owner.Creature.CombatState);
        }

        var executedCommand = await command.Execute(choiceContext);

        var targets = executedCommand.Results
            .SelectMany(r => r)
            .Select(r => r.Receiver)
            .Distinct()
            .ToList();

        AmmoResource.InvokeOnAmmoFired(Owner, targets);
    }

    protected override void OnUpgrade()
    {
    }

    public override TargetType TargetType => TargetType.RandomEnemy;
}