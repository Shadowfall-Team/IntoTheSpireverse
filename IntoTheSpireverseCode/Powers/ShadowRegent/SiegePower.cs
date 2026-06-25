using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class SiegePower : ShadowPowerModel, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public async Task OnAmmoFired(Player player, IEnumerable<List<DamageResult>> results)
    {
        if (player.Creature != Owner) return;

        Flash();

        var targets = results
            .SelectMany(r => r)
            .Select(r => r.Receiver)
            .Distinct()
            .ToList();

        foreach (var target in targets.Where(t => t.IsAlive))
        {
            await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, Amount, Owner, null);
            await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, Amount, Owner, null);
        }

        await PowerCmd.Remove(this);
    }
}