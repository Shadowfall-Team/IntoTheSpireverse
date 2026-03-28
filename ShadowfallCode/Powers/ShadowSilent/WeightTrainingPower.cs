using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public sealed class WeightTrainingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        var enemies = CombatState.HittableEnemies;
        if (enemies.Count == 0) return;

        Creature? target = Owner.Player?.RunState.Rng.CombatTargets.NextItem(enemies);
        Flash();
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, base.Amount, ValueProp.Unpowered, base.Owner, null);
    }
}
