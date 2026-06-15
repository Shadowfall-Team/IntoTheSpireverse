using BaseLib.Abstracts;
using BaseLib.Cards;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
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
        .. MakeCalculatedBlock("ShotBlock", 0, (model, _) => GetOwnerBlockadeAmount(model))
    ];

    public override bool GainsBlock => GetOwnerBlockadeAmount(this) > 0;

    private static decimal GetOwnerBlockadeAmount(CardModel cardModel)
    {
        if (!cardModel.IsMutable || cardModel.Pile == null)
        {
            return 0;
        }

        return cardModel.Owner.Creature.GetPowerInstances<DefensiveCannonadePower>().Sum(p => p.Amount);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [BaseLibKeywords.Purge];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hasBigGuns = Owner.Creature.HasPower<BigGunsPower>();

        Creature? pickedTarget = null;
        if (!hasBigGuns)
        {
            var hittableEnemies = CombatState?.HittableEnemies.ToList();
            if (hittableEnemies?.Count > 0)
            {
                pickedTarget = Owner.RunState.Rng.CombatTargets.NextItem(hittableEnemies);
            }
        }

        if (!hasBigGuns && pickedTarget == null)
            return;

        await ShotHelper.CreateMissile(CombatState, pickedTarget);

        if (GainsBlock)
        {
            await CreatureCmd.GainBlock(Owner.Creature,
                ((CalculatedVar)DynamicVars["ShotBlock"]).Calculate(Owner.Creature), ValueProp.Move, cardPlay);
        }

        var baseDamage = DynamicVars.CalculationBase.BaseValue;
        var extraDamage = DynamicVars.ExtraDamage.BaseValue;
        var multiplier = Owner.Creature.GetPowerAmount<NextVolleyDamagePower>()
                         + Owner.Creature.GetPowerAmount<VolleyDamagePower>();
        var damage = baseDamage + extraDamage * multiplier;

        var command = DamageCmd.Attack(damage)
                .WithHitCount(1)
                .FromCard(this)
                .WithAttackerAnim("Cast", Owner.Character.AttackAnimDelay)
                .WithHitFx("vfx/vfx_starry_impact", null, "blunt_attack.mp3")
            // .WithAttackerFx(null, "event:/sfx/characters/regent/regent_sovereign_blade", null)
            ;

        if (hasBigGuns)
        {
            command.TargetingAllOpponents(Owner.Creature.CombatState);
        }
        else
        {
            command.Targeting(pickedTarget!);
        }

        var executedCommand = await command.Execute(choiceContext);

        await AmmoResource.InvokeOnAmmoFired(Owner, executedCommand.Results);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
    }

    public override TargetType TargetType => TargetType.RandomEnemy;
}