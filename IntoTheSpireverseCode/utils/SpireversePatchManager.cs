using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches.CombatPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches.Input;
using IntoTheSpireverse.Patches;

namespace IntoTheSpireverse.IntoTheSpireverseCode.utils;

public class SpireversePatchManager
{
    public static void HarmonyPatches()
    {
        var patcher = ModPatcher.Create(MainFile.ModId, MainFile.Logger)
            .Add(typeof(NCardPileScreenReadyPatch))
            .Add(typeof(NCombatPilesContainerPatch))
            .Add(typeof(NCombatUiActivatePatch))
            .Add(typeof(NCreaturePatch))
            .Add(typeof(NEndTurnButtonPatch))
            .Add(typeof(NButtonPatches))
            .Add(typeof(NHotkeyManagerPatches))
            .Add(typeof(NInputManagerPatches))
            .Add(typeof(ArtHoverTipColorPatch))
            .Add(typeof(CardFactoryTransformPatch))
            .Add(typeof(CardGlowGoldListenerPatch))
            .Add(typeof(CardModifierPreviewPatch))
            .Add(typeof(CustomOrbCreateSprite))
            .Add(typeof(CustomOrbIcon))
            .Add(typeof(CustomOrbIconPath))
            .Add(typeof(CustomOrbSpritePath))
            .Add(typeof(EnchantBlockWithoutCardPlayPatch))
            .Add(typeof(GiantRockPatches))
            .Add(typeof(GiantRockDowngradePatch))
            .Add(typeof(HandPositionTrackingPatch))
            .Add(typeof(HandPositionTrackingCleanupPatch))
            .Add(typeof(InciteViolencePatch))
            .Add(typeof(LingerDiscardRedirectPatch))
            .Add(typeof(LingerHasTurnEndPatch))
            .Add(typeof(NCardModificationFlagPatch))
            .Add(typeof(NCharacterSelectButtonPatches))
            .Add(typeof(NCharacterSelectScreenPatches))
            .Add(typeof(OrbCmdSlotPatch))
            .Add(typeof(OrbModelPatch))
            .Add(typeof(OvercostListenerPatch))
            .Add(typeof(BeforeEnergySpentListenerPatch))
            .Add(typeof(RockTransformPatches.RockTransformFromPatch))
            .Add(typeof(RockTransformPatches.RockTransformToPatch))
            .Add(typeof(VisualCardPoolPatches));
        
        if (GameVersion.HasCardLocation)
            patcher.Add(typeof(ModifyCardPlayResultLocationNewPatch))
                .Add(typeof(AfterModifyingLocationNewPatch));
        else
            patcher.Add(typeof(ModifyCardPlayResultLocationOldPatch))
                .Add(typeof(AfterModifyingLocationOldPatch));
        
        patcher.PatchAll();
    }
}