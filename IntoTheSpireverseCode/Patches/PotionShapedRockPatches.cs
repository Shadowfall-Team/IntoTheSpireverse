using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Exists for Gabbro: "Rocks deal additional damage" includes Potion-Shaped Rock. Both patches read
/// <see cref="GabbroPower.PotionRockBonus"/>, so the tooltip and the hit cannot drift apart.
/// </summary>
public static class PotionShapedRockPatches
{
    /// <summary>
    /// The rock's hit carries no cardSource, so a damage modifier cannot tell it apart from the
    /// owner's other sourceless damage without tracking "a rock potion is resolving" state. The bonus
    /// is added to the potion's own hit instead.
    ///
    /// Mirrors the vanilla OnUse body, and only takes over while there is a bonus to add.
    /// </summary>
    [HarmonyPatch(typeof(PotionShapedRock), "OnUse")]
    public static class DamagePatch
    {
        static bool Prefix(PotionShapedRock __instance, PlayerChoiceContext choiceContext, Creature? target,
            ref Task __result)
        {
            var bonus = GabbroPower.PotionRockBonus(__instance);
            if (bonus == 0m) return true;

            ArgumentNullException.ThrowIfNull(target);
            var damage = __instance.DynamicVars.Damage;
            __result = CreatureCmd.Damage(choiceContext, target, damage.BaseValue + bonus, damage.Props,
                __instance.Owner.Creature);
            return false;
        }
    }

    /// <summary>
    /// The belt and the potion popup both rebuild the text from DynamicDescription on every hover, so
    /// it tracks Gabbro without any refresh.
    ///
    /// Canonical potions are skipped: they have no owner, and HoverTipFactory.FromPotion caches their
    /// tip, which is why Alkalize's preview keeps showing the base damage.
    /// </summary>
    [HarmonyPatch(typeof(PotionModel), nameof(PotionModel.DynamicDescription), MethodType.Getter)]
    public static class TooltipPatch
    {
        static void Postfix(PotionModel __instance, LocString __result)
        {
            var bonus = GabbroPower.PotionRockBonus(__instance);
            if (bonus == 0m) return;

            __result.Add("Damage", __instance.DynamicVars.Damage.BaseValue + bonus);
        }
    }
}
