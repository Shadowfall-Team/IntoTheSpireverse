using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class GrapeshotPower : ShadowPowerModel, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnAmmoFired(Player player, IEnumerable<List<DamageResult>> results)
    {
        if (player.Creature != Owner) return;
        Flash();

        var resultsList = results.ToList();
        for (var i = 0; i < Amount; i++)
        {
            foreach (List<DamageResult> result in resultsList)
            {
                IReadOnlyList<Creature>? hitTargets;
                if (Owner.HasPower<BigGunsPower>())
                {
                    hitTargets = CombatState.HittableEnemies;
                    await ShotHelper.CreateMissile(CombatState, null);
                }
                else
                {
                    var hittableEnemies = CombatState.HittableEnemies.ToList();
                    var preferredTargets = hittableEnemies.Where(e => e.HasPower<TargetedPower>()).ToList();
                    var targetPool = preferredTargets.Count > 0 ? preferredTargets : hittableEnemies;
                    hitTargets =
                        [Owner.Player.RunState.Rng.CombatTargets.NextItem(targetPool)];
                    await ShotHelper.CreateMissile(CombatState, hitTargets[0]);
                }

                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), hitTargets,
                    result.Max(d => d.TotalDamage) / 2M, ValueProp.Unpowered, Owner);
            }
        }
    }
}