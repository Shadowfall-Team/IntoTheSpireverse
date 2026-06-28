using BaseLib.Abstracts;
using BaseLib.Cards;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public class AmmoVolley() : CustomCardModel(1, CardType.Attack, CardRarity.Token, TargetType.RandomEnemy)
{
    public override string CustomPortraitPath => $"res://IntoTheSpireverse/images/card_portraits/regent/big/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    public static readonly string ShotBlockKey = "ShotBlock";
    public static readonly string GrapeshotDamageKey = "GrapeshotDamage";
    public static readonly string GrapeshotHitsKey = "GrapeshotHits";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [BaseLibKeywords.Purge];

    private class CalculatedGrapeshotHitsVar() : CalculatedVar(GrapeshotHitsKey)
    {
        protected override DynamicVar GetBaseVar()
        {
            return ((CardModel)_owner).DynamicVars[GrapeshotHitsKey + "Base"];
        }

        protected override DynamicVar GetExtraVar()
        {
            return ((CardModel)_owner).DynamicVars[GrapeshotHitsKey + "Extra"];
        }
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14, ValueProp.Move),
        .. MakeCalculatedBlock(ShotBlockKey, 0, (model, _) => GetOwnerBlockadeAmount(model)),
        // can't use make calculated damage because it only accepts int for the base and extra
        new CalculationBaseVar(0m),
        new ExtraDamageVar(0.5m),
        new CalculatedDamageVar(ValueProp.Unpowered).WithMultiplier(CalculateHalvedDamage),
        new DynamicVar(GrapeshotHitsKey + "Base", 0m),
        new DynamicVar(GrapeshotHitsKey + "Extra", 1m),
        new CalculatedGrapeshotHitsVar().WithMultiplier((card, _) => GetOwnerGrapeshotAmount(card))
    ];

    protected override void AddExtraArgsToDescription(LocString description)
    {
        var hasGrapeshot = Owner?.HasPower<GrapeshotPower>() == true;
        var hasBigGuns = Owner?.HasPower<BigGunsPower>() == true;
        description.Add("HasGrapeshot", hasGrapeshot);
        description.Add("HasBigGuns", hasBigGuns);
    }

    public static decimal CalculateHalvedDamage(CardModel card, Creature? creature)
    {
        if (card.Owner is null) return 0m;
        return Hook.ModifyDamage(
            card.Owner.RunState,
            card.Owner.Creature.CombatState,
            creature,
            card.Owner.Creature,
            card.DynamicVars.Damage.BaseValue,
            ValueProp.Move,
            card,
            ModifyDamageHookType.Additive | ModifyDamageHookType.Multiplicative,
            CardPreviewMode.None,
            out _
        );
    }

    public override bool GainsBlock => GetOwnerBlockadeAmount(this) > 0;

    private static decimal GetOwnerBlockadeAmount(CardModel cardModel)
    {
        if (!cardModel.IsMutable || cardModel.Pile == null)
        {
            return 0;
        }

        return cardModel.Owner.Creature.GetPowerInstances<DefensiveCannonadePower>().Sum(p => p.Amount);
    }

    private static decimal GetOwnerGrapeshotAmount(CardModel cardModel)
    {
        if (!cardModel.IsMutable || cardModel.Pile == null) return 0;
        if (cardModel.Owner is null) return 0;

        return cardModel.Owner.Creature.GetPowerInstances<GrapeshotPower>().Sum(p => p.Amount);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null) { return; }
        var hasBigGuns = Owner.Creature.HasPower<BigGunsPower>();

        Creature? pickedTarget = null;
        if (!hasBigGuns)
        {
            var hittableEnemies = CombatState?.HittableEnemies.ToList();
            if (hittableEnemies?.Count > 0)
            {
                var preferredTargets = hittableEnemies.Where(e => e.HasPower<TargetedPower>()).ToList();
                var targetPool = preferredTargets.Count > 0 ? preferredTargets : hittableEnemies;
                pickedTarget = Owner.RunState.Rng.CombatTargets.NextItem(targetPool);
            }
            if (pickedTarget == null)
                return;
        }

        await ShotHelper.CreateMissile(CombatState, pickedTarget);

        // block before attack? maybe not consistent with the order in the description
        if (GainsBlock)
        {
            await CreatureCmd.GainBlock(Owner.Creature,
                ((CalculatedVar)DynamicVars[ShotBlockKey]).Calculate(Owner.Creature), ValueProp.Move, cardPlay);
        }

        var command = DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .WithAttackerAnim("Cast", Owner.Character.CastAnimDelay)
                .WithHitFx("vfx/vfx_starry_impact", null, "blunt_attack.mp3")
            // .WithAttackerFx(null, "event:/sfx/characters/regent/regent_sovereign_blade", null)
            ;

        if (hasBigGuns)
        {
            command = command.TargetingAllOpponents(CombatState);
        }
        else if (pickedTarget?.IsHittable == true)
        {
            command = command.Targeting(pickedTarget);
        }

        var executedCommand = await command.Execute(choiceContext);

        // Grapeshot secondary N hits
        // Vigor doesn't persist currently
        if (Owner.Creature.HasPower<GrapeshotPower>())
        {
            for (int i = 0; i < ((CalculatedVar)DynamicVars[GrapeshotHitsKey]).Calculate(null); i++)
            {
                var targets = hasBigGuns ? CombatState.HittableEnemies : [pickedTarget];

                // iterating targets because the hooks need to run for each one (maybe?)
                foreach (var target in CombatState.HittableEnemies)
                {
                    // with advice from OLC
                    var damage = 0.5m * Hook.ModifyDamage(
                        Owner.RunState,
                        Owner.Creature.CombatState,
                        target,
                        Owner.Creature,
                        DynamicVars.Damage.BaseValue,
                        ValueProp.Move,
                        this,
                        ModifyDamageHookType.Additive | ModifyDamageHookType.Multiplicative,
                        CardPreviewMode.None,
                        out _
                    );
                    damage = Math.Floor(damage);

                    await ShotHelper.CreateMissile(CombatState, target, true);

                    await DamageCmd.Attack(damage)
                        .FromCard(this)
                        .Unpowered()
                        .WithHitFx("vfx/vfx_starry_impact", null, "blunt_attack.mp3")
                        .Targeting(target)
                        .WithNoAttackerAnim()
                        .Execute(choiceContext);
                }
            }
        }

        await AmmoResource.InvokeOnAmmoFired(Owner, executedCommand.Results);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
    }

    public override TargetType TargetType => TargetType.RandomEnemy;
}
