using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class CorpseExplosionPower : ShadowPowerModel, IAfterDeathCompat
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 111 = (PlayerChoiceContext, Creature, bool, float)
    // public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    //
    // 113 = (PlayerChoiceContext, Creature, CardModel?, bool, float)
    public async Task AfterDeathCompat(PlayerChoiceContext choiceContext, Creature creature,
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
