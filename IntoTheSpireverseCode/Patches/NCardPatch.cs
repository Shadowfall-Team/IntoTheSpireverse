using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace IntoTheSpireverse.Patches;

[HarmonyPatch(typeof(NCard), "Reload")]
public static class NCardPatch
{
    static void Postfix(NCard __instance)
    {
        if (__instance == null || !__instance.IsNodeReady() || __instance.Model == null)
            return;
        
        string cardId = __instance.Model.Id.ToString();

        float h = 1f, s = 1f, v = 1f;
        float r = 1f, g = 1f, b = 1f;
        float contrast = 1f;
        bool flipH = false;

        var data = CardArtRoller.GetCardData(cardId) ?? CardArtRoller.GetDefaultHsvForCard(cardId);
        if (data != null)
        {
            h        = data.Hue;
            s        = data.Saturation;
            v        = data.Value;
            r        = data.Red;
            g        = data.Green;
            b        = data.Blue;
            contrast = data.Contrast;
            flipH    = data.FlipH;
        }

        if (__instance.Model.Rarity == CardRarity.Ancient)
        { 
            var ancientPortrait = __instance.GetNodeOrNull<TextureRect>("%AncientPortrait");
            CardShaderHelper.ApplyToPortrait(ancientPortrait, h, s, v, r, g, b, contrast);
            ancientPortrait.FlipH = flipH;
        }
        else
        {
            var portrait = __instance.GetNodeOrNull<TextureRect>("%Portrait");
            CardShaderHelper.ApplyToPortrait(portrait, h, s, v, r, g, b, contrast);
            portrait.FlipH = flipH;
        }




    }
}