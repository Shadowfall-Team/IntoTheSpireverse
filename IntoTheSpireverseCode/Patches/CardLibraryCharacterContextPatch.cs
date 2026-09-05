using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Tracks which character's pool the Card Library is currently filtered to.
///
/// Compendium cards are canonical and have no owner, so there is otherwise no way to tell that the
/// player is looking at the Tectonic's pool rather than the Ironclad's. BaseLib registers custom
/// characters into NCardLibrary's private _cardPoolFilters, so the selected filter is a reliable
/// answer once it can be read back out.
/// </summary>
public static class CardLibraryCharacterContextPatch
{
    private static NCardLibrary? _library;
    private static CharacterModel? _viewed;

    /// <summary>
    /// The character whose pool is on screen, or null when the library is not open.
    ///
    /// Deliberately gated on the library still being alive and in the tree: without that, a filter
    /// left selected would keep colouring canonical cards long after the screen was closed.
    /// </summary>
    public static CharacterModel? ViewedCharacter =>
        _library != null && GodotObject.IsInstanceValid(_library) && _library.IsInsideTree()
            ? _viewed
            : null;

    [HarmonyPatch(typeof(NCardLibrary), nameof(NCardLibrary._Ready))]
    public static class Opened
    {
        static void Postfix(NCardLibrary __instance)
        {
            _library = __instance;
            Refresh(__instance);
        }
    }

    [HarmonyPatch(typeof(NCardLibrary), "UpdateCardPoolFilter")]
    public static class FilterChanged
    {
        static void Postfix(NCardLibrary __instance)
        {
            _library = __instance;
            Refresh(__instance);
        }
    }

    /// <summary>
    /// Opening the library inside a run preselects the run character's pool filter by assigning
    /// NCardPoolFilter.IsSelected directly, which does not emit Toggled - so UpdateCardPoolFilter never
    /// runs and the filter change would otherwise go unseen. From the main menu there is no run
    /// character, the Ironclad tab is preselected, and reaching another tab means clicking it, which
    /// does emit; that is why only the in-run case needs this.
    /// </summary>
    [HarmonyPatch(typeof(NCardLibrary), nameof(NCardLibrary.OnSubmenuOpened))]
    public static class SubmenuOpened
    {
        static void Postfix(NCardLibrary __instance)
        {
            _library = __instance;
            Refresh(__instance);
        }
    }

    private static void Refresh(NCardLibrary library)
    {
        _viewed = null;

        if (AccessTools.DeclaredField(typeof(NCardLibrary), "_cardPoolFilters")
                ?.GetValue(library) is not Dictionary<CharacterModel, NCardPoolFilter> filters)
            return;

        foreach (var (character, filter) in filters)
        {
            if (filter == null || !GodotObject.IsInstanceValid(filter) || !filter.IsSelected) continue;
            _viewed = character;
            return;
        }
    }
}
