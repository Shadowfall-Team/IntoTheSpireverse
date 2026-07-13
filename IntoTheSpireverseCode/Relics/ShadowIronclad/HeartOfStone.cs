using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowIronclad;

public class HeartOfStone : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(18m),
        new MaxHpVar(2m),
    ];

    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<HeartOfTheMountain>();

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        //TODO: New Starter Relic: Absorbs the first 6 HP you lose each combat.
        //TODO: New Starter Upgrade: Absorbs the first 12 HP you lose each combat.

        if (room.RoomType != RoomType.Elite) return;
        if (Owner.Creature.IsDead) return;

        Flash();
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
    }
}
