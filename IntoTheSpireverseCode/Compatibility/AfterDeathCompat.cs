using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

[HarmonyPatch]
internal static class AfterDeathCompat
{
    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterDeath))]
    [HarmonyPostfix]
    internal static Task AfterDeathPostfix(Task __result, AbstractModel __instance, object[] __args)
    {
        if (__instance is IAfterDeathCompat afterDeathCompat)
        {
            // Awful awful code here
            int i = 0;
            var choiceContext = __args[i] as PlayerChoiceContext;
            var creature = __args[++i] as Creature;
            CardModel? source = null;
            if (__args.Length == 5)
            {
                source = __args[++i] as CardModel;
            }
            var wasRemovalPrevented = (bool)__args[++i];
            var deathAnimLength = (float)__args[++i];

            return AfterDeath(__result, afterDeathCompat, choiceContext!, creature!, source, wasRemovalPrevented, deathAnimLength);
        }
        return __result;
    }

    private static async Task AfterDeath(Task original, IAfterDeathCompat instance,
        PlayerChoiceContext choiceContext, Creature creature, CardModel? source, bool wasRemovalPrevented, float deathAnimLength)
    {
        await original;
        await instance.AfterDeathCompat(choiceContext, creature, source, wasRemovalPrevented, deathAnimLength);
    }
}

public interface IAfterDeathCompat
{
    abstract Task AfterDeathCompat(PlayerChoiceContext choiceContext, Creature creature, CardModel? source, bool wasRemovalPrevented, float deathAnimLength);
}
