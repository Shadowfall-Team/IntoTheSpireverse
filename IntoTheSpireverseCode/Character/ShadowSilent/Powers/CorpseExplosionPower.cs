using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class CorpseExplosionPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 107/111 = (PlayerChoiceContext, Creature, bool, float)
    // public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    //
    // 113     = (PlayerChoiceContext, Creature, CardModel?, bool, float)
    // TODO: add compat hook call
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        CardModel? source, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (Applier == null) return;
        if (creature != Owner || wasRemovalPrevented)
            return;

        Flash();
        decimal damage = (creature.MaxHp * Amount);
        await CreatureCmd.Damage(
            choiceContext, CombatState.HittableEnemies,
            damage, ValueProp.Unpowered,
            Applier);
    }
}