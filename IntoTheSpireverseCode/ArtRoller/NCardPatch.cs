using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ArtRoller;

[HarmonyPatch(typeof(NCard), "Reload")]
public static class NCardPatch
{
    /// <summary>
    /// Clears the mirror before every reload. NCard nodes are reused and the base game never resets
    /// FlipH, so a card that resolves to no roll would otherwise inherit the flip of whatever the
    /// node showed last and render mirrored (0daf1adc). This is the flip's only unconditional
    /// reset, which is why it must stay outside any "is this our card" filtering.
    /// </summary>
    internal static void Prefix(NCard __instance)
    {
        if (!__instance.IsNodeReady() || __instance.Model is not {} model)
            return;

        if (model.Rarity == CardRarity.Ancient)
        {
            if (__instance.GetNodeOrNull<TextureRect>("%AncientPortrait") is not {} ancientPortrait) return;
            ancientPortrait.FlipH = false;
        }
        else
        {
            if (__instance.GetNodeOrNull<TextureRect>("%Portrait") is not {} portrait) return;
            portrait.FlipH = false;
        }
    }

    [HarmonyPriority(Priority.Last)]
    internal static void Postfix(NCard __instance)
    {
        if (!__instance.IsNodeReady() || __instance.Model is null)
            return;

        // Scoping is resolved centrally so the colour grading and the portrait override always
        // agree about which roll applies to this card.
        //
        // Do not re-add a "only this mod's card types" test here. One existed (14658a9d, "improve
        // ArtRoller compatibility with other mods") and it is why the compendium stopped showing
        // recoloured reprints: a base game card reprinted into an alt character's pool is still the
        // base game's type, so the test skipped exactly the cards the scoped rolls exist for. It
        // was written against the pre-scoping code, which looked up a plain card id and so really
        // could bleed onto anyone's cards, and it survived a merge onto the scoped version where it
        // only does harm.
        //
        // It is not what stops cards being mirrored, either - that is the Prefix above, which
        // resets FlipH for every card unconditionally (0daf1adc).
        //
        // What keeps other mods and the vanilla characters clean is Resolve: both lookups behind it
        // are exact-key, a dictionary hit and a file probe, so a card with no roll of its own
        // resolves to null, and a base game card resolves to its scoped roll only while an alt
        // character is the one showing it.
        //
        // The neutral 1f defaults below still matter: a card that resolves to no roll has to
        // actively reset the shader rather than return early, for the same node-reuse reason.

        float h = 1f, s = 1f, v = 1f;
        float r = 1f, g = 1f, b = 1f;
        float contrast = 1f;
        bool flipH = false;

        var data = CardArtRoller.Resolve(__instance.Model);
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
            if (__instance.GetNodeOrNull<TextureRect>("%AncientPortrait") is not {} ancientPortrait) return;
            CardShaderHelper.ApplyToPortrait(ancientPortrait, h, s, v, r, g, b, contrast);
            ancientPortrait.FlipH = flipH;
        }
        else
        {
            if (__instance.GetNodeOrNull<TextureRect>("%Portrait") is not {} portrait) return;
            CardShaderHelper.ApplyToPortrait(portrait, h, s, v, r, g, b, contrast);
            portrait.FlipH = flipH;
        }
    }
}