using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.utils;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Nodes.Screens.PotionLab;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;

public static class Artists
{
    public static readonly ArtistInfo AnneBean = new()
    {
        Name = "AnneBean",
        Links = ["https://linktr.ee/annebean"],
    };
}

namespace IntoTheSpireverse.IntoTheSpireverseCode.utils
{
    public sealed class ArtistInfo
    {
        public required string Name { get; init; }
        public string[] Links { get; init; } = [];

        public string Description => string.Join("\n", Links.Select(url => $"[url]{url}[/url]"));
    }

    public static class ArtistHoverTipUtils
    {
        public static readonly HashSet<string> ArtistTipIds = [];

        private static bool IsInspectingOrInCompendium(AbstractModel model)
        {
            bool mainCompendium = (
                NGame.Instance?.MainMenu?.SubmenuStack?._compendiumSubmenu?._stack._submenus ?? []
            ).Any(m => m is NCardLibrary or NRelicCollection or NPotionLab);
            bool runCompendium =
                NRun.Instance?.GlobalUi?.SubmenuStack?.Stack != null
                && (
                    NRun.Instance.GlobalUi.SubmenuStack.Stack._cardLibrarySubmenu != null
                    || NRun.Instance.GlobalUi.SubmenuStack.Stack._relicCollectionSubmenu != null
                    || NRun.Instance.GlobalUi.SubmenuStack.Stack._potionLabSubmenu != null
                );

            bool inspecting =
                (NGame.Instance?.InspectCardScreen?._card?._model == model)
                || (NGame.Instance?.InspectRelicScreen?._relics.Contains(model) == true);
            return inspecting || mainCompendium || runCompendium;
        }

        public static IEnumerable<IHoverTip> HoverTip(this ArtistInfo info, AbstractModel model)
        {
            if (!IsInspectingOrInCompendium(model))
                yield break;

            var locString = new LocString(
                "static_hover_tips",
                "INTOTHESPIREVERSE-ARTIST_HOVER_TIP.title"
            );
            locString.Add(new StringVar("ArtistName", info.Name));

            var icon = PreloadManager.Cache.GetAsset<Texture2D>(
                "res://images/atlases/ui_atlas.sprites/cursor_pencil.tres"
            );
            HoverTip tip = new(locString, info.Description, icon)
            {
                Id = $"{MainFile.ModId}-{info.Name}",
            };
            ArtistTipIds.Add(tip.Id);

            yield return tip;
        }
    }
}
