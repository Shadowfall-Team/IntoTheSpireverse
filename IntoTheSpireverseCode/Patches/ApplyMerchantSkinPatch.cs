using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode;

[HarmonyPatch(typeof(NMerchantRoom), "AfterRoomIsLoaded")]
public static class ApplyMerchantSkinPatch
{
    public static void Postfix(NMerchantRoom __instance)
    {
        try
        {
            var players = Traverse.Create(__instance).Field("_players").GetValue<List<Player>>();
            var visuals = __instance.PlayerVisuals;
            if (players == null)
                return;

            var count = Math.Min(players.Count, visuals.Count);
            for (var i = 0; i < count; i++)
            {
                var characterType = players[i].Character?.GetType();
                if (characterType != null)
                    SkinManager.ApplyMerchantSkin(visuals[i], characterType);
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Failed to apply merchant skin: {e}");
        }
    }
}
