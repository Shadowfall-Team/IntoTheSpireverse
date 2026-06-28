using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public static class ShotHelper
{
    public static async Task CreateMissile(ICombatState combatState, Creature? pickedTarget, bool skipWait = false)
    {
        var combatRoom = NCombatRoom.Instance;
        if (combatRoom != null)
        {
            var missileTarget = pickedTarget != null
                ? combatRoom.GetCreatureNode(pickedTarget)?.GetBottomOfHitbox()
                : VfxCmd.GetSideCenterFloor(CombatSide.Enemy, combatState);

            if (missileTarget is { } pos)
            {
                var missile = NSmallMagicMissileVfx.Create(pos, new Color("c01020"));
                if (missile != null)
                {
                    combatRoom.CombatVfxContainer.AddChildSafely(missile);
                    if (skipWait)
                        return;
                    await Cmd.Wait(missile.WaitTime);
                }
            }
        }
    }
}