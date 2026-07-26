using BaseLib.Extensions;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Commands;

/// <summary>
/// Resolves Ammo shots.
///
/// This is a Command and not a GameAction on purpose. Per StS2's own GameAction docs, a
/// GameAction wraps *player input* and the Commands it calls carry the game logic;
/// <see cref="Ammo.FireAmmoAction"/> is that input wrapper for the FIRE button and nothing more.
/// Anything already running inside a networked action - a card's OnPlay, a power's hook - calls
/// this directly instead. Enqueueing a second GameAction from inside one can create a MP sync problem.
/// </summary>
public static class FireAmmoCmd
{
    /// <summary>
    /// Resolves one Ammo shot: spends the Ammo, fires the missile and runs every on-fire
    /// effect. Returns false if the shot could not happen at all - no Ammo, not enough energy
    /// when charging, or nothing left alive to shoot at.
    ///
    /// <paramref name="bonusDamage"/> is added on top of the normal shot value for this shot
    /// only - a per-shot bonus the caller owns, not a standing power. It counts as part of the
    /// shot, so Grapeshot's follow-up hits (50% of shot damage) scale off the bonused value too.
    /// </summary>
    public static async Task<bool> Fire(Player player, bool chargeEnergy, decimal bonusDamage = 0)
    {
        if (CombatManager.Instance.IsOverOrEnding) return false;

        var combatState = player.Creature.CombatState;
        if (combatState == null) return false;

        var cost = chargeEnergy ? AmmoResource.GetShotEnergyCost(player) : 0;
        var hasBigGuns = player.Creature.HasPower<MassMunitionPower>();

        if (AmmoResource.GetAmmo(player) <= 0 || player.PlayerCombatState?.Energy < cost ||
            !hasBigGuns && !combatState.HittableEnemies.Any())
        {
            return false;
        }

        // Doubles as the cardSource on every damage call below, which is how powers such as
        // PiercedPower tell an Ammo shot apart from other Unpowered damage.
        var phantomCard = AmmoResource.GetOrCreatePhantomCard(player);

        if (chargeEnergy)
        {
            await PlayerCmd.LoseEnergy(cost, player);
            if (phantomCard != null)
                await Hook.AfterEnergySpent(combatState, phantomCard, cost);
        }

        AmmoResource.LoseAmmo(1, player);
        await AmmoResource.InvokeOnAmmoFiring(player);

        Creature? pickedTarget = null;
        if (!hasBigGuns)
            pickedTarget = AmmoResource.PickShotTarget(player, combatState);

        await ShotHelper.CreateMissile(combatState, pickedTarget);

        var blockAmount = combatState.IterateHookListeners()
            .OfType<DefensiveCannonadePower>()
            .Where(p => p.Owner == player.Creature)
            .Sum(p => p.Amount);
        if (blockAmount > 0)
        {
            await CreatureCmd.GainBlock(player.Creature, blockAmount, ValueProp.Move, null);
        }

        var shotDamage = AmmoResource.GetShotDamage(player) + bonusDamage;
        IEnumerable<Creature> targets = hasBigGuns
            ? combatState.HittableEnemies
            : (IEnumerable<Creature>)[pickedTarget!];

        var results = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
            targets, shotDamage, ValueProp.Unpowered, player.Creature, phantomCard, null);

        if (player.Creature.HasPower<GrapeshotPower>())
        {
            var grapeshot = player.Creature.GetPowerAmount<GrapeshotPower>();
            // Half of the full shot, bonus included, so the bonus rides along on every extra hit.
            var halfDmg = Math.Floor(0.5m * shotDamage);
            for (var i = 0; i < grapeshot; i++)
            {
                if (hasBigGuns)
                {
                    await ShotHelper.CreateMissile(combatState, null, skipWait: true);
                    foreach (var t in combatState.HittableEnemies)
                        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
                            t, halfDmg, ValueProp.Unpowered, player.Creature, phantomCard, null);
                }
                else
                {
                    var followTarget = AmmoResource.PickShotTarget(player, combatState);
                    await ShotHelper.CreateMissile(combatState, followTarget, skipWait: true);
                    if (followTarget == null) continue;
                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
                        followTarget, halfDmg, ValueProp.Unpowered, player.Creature, phantomCard, null);
                }
            }
        }

        await AmmoResource.InvokeOnAmmoFired(player, [results.ToList()]);
        return true;
    }

    /// <summary>
    /// Fires up to <paramref name="shots"/> Ammo in sequence, stopping at the first shot that
    /// cannot resolve. That is not only "the stockpile ran dry": killing the last enemy with an
    /// early shot ends the volley too, since a shot with no target cannot happen.
    /// <paramref name="bonusDamage"/> is applied to every shot in the volley. Returns how many
    /// shots actually fired.
    /// </summary>
    public static async Task<int> FireVolley(
        int shots, Player player, bool chargeEnergy, decimal bonusDamage = 0)
    {
        var fired = 0;
        for (var i = 0; i < shots; i++)
        {
            if (!await Fire(player, chargeEnergy, bonusDamage)) break;
            fired++;
        }

        return fired;
    }
}
