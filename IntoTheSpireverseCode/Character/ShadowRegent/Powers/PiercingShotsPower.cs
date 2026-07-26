using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;

public class PiercingShotsPower : ShadowPowerModel, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

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
            await PowerCmd.Apply<PiercedPower>(
                new ThrowingPlayerChoiceContext(), target, Amount, Owner, null);
        }
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        // Owner is the player, so their own side ending is this buff's "end of turn".
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
