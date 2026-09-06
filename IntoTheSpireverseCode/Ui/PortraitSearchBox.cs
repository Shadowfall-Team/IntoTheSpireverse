using Godot;
using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Logging;

namespace IntoTheSpireverse;

/// <summary>
/// Manages the searchable portrait path dropdown in the card inspector.
/// </summary>
public class PortraitSearchBox
{
    private readonly LineEdit _searchBox;
    private readonly ItemList _searchList;
    private readonly List<string> _portraits;

    public event Action<string>? PortraitSelected;

    public string Text
    {
        get => _searchBox.Text;
        set => _searchBox.Text = value;
    }

    public PortraitSearchBox(Godot.Node parent)
    {
        _portraits = LoadAllPortraitPaths();

        _searchBox = new LineEdit();
        _searchBox.PlaceholderText = "Search portrait paths...";
        _searchBox.CustomMinimumSize = new Vector2(300, 40);
        _searchBox.TextChanged += OnSearchTextChanged;
        parent.AddChild(_searchBox);

        _searchList = new ItemList();
        _searchList.ItemSelected += OnItemSelected;
        _searchList.TopLevel = true;
        _searchList.Hide();

        var bgStyle = new StyleBoxFlat();
        bgStyle.BgColor = new Color(0.08f, 0.08f, 0.08f, 0.95f);
        bgStyle.BorderColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        bgStyle.SetBorderWidthAll(1);
        _searchList.AddThemeStyleboxOverride("panel", bgStyle);

        _searchBox.AddChild(_searchList);

        Log.Info($"[PortraitSearchBox] Loaded {_portraits.Count} portrait paths.");
        
        
    }

    private void OnSearchTextChanged(string searchText)
    {
        _searchList.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            _searchList.Hide();
            return;
        }

        int count = 0;
        foreach (string path in _portraits)
        {
            string display = BuildDisplayName(path);
            
            if (display.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                _searchList.AddItem(display);
                _searchList.SetItemMetadata(count, path);
                count++;
                if (count >= 15) break;
            }
        }

        if (count > 0)
        {
            Vector2 globalPos = _searchBox.GlobalPosition;
            _searchList.GlobalPosition = new Vector2(globalPos.X, globalPos.Y - 250);
            _searchList.Size = new Vector2(_searchBox.Size.X, 250);
            _searchList.Show();
        }
        else
        {
            _searchList.Hide();
        }
    }

    private void OnItemSelected(long index)
    {
        string selectedPath = (string)_searchList.GetItemMetadata((int)index);

        string name = System.IO.Path.GetFileNameWithoutExtension(selectedPath);
        string folder = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(selectedPath)) ?? "";
        _searchBox.Text = string.IsNullOrEmpty(folder) ? name : $"{folder}/{name}";

        _searchList.Hide();
        PortraitSelected?.Invoke(selectedPath);
    }

    /// <summary>
    /// Where the mod keeps its own card art. <see cref="IntoTheSpireverseCard.CustomPortraitPath"/>
    /// resolves to the <c>big/</c> variant, so that is the one the roller offers.
    /// </summary>
    private static string CustomPortraitRoot => $"res://{MainFile.ModId}/images/card_portraits";

    /// <summary>
    /// "folder/name" for the picker list. Custom art lives one level deeper than base game art
    /// (<c>card_portraits/ironclad/big/foo.png</c>), so naming the immediate folder would label
    /// every custom portrait "big/" and lose the character it belongs to. Step over "big" so the
    /// character folder is shown instead.
    /// </summary>
    private static string BuildDisplayName(string path)
    {
        string name = System.IO.Path.GetFileNameWithoutExtension(path);
        string? folderPath = System.IO.Path.GetDirectoryName(path);
        string folder = System.IO.Path.GetFileName(folderPath) ?? "";

        if (folder.Equals("big", StringComparison.OrdinalIgnoreCase))
            folder = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(folderPath)) ?? folder;

        return string.IsNullOrEmpty(folder) ? name : $"{folder}/{name}";
    }

    private static List<string> LoadAllPortraitPaths()
    {
        var paths = new List<string>();
        try
        {
            foreach (var card in MegaCrit.Sts2.Core.Models.ModelDb.AllCards)
            {

                if (!paths.Contains(card.PortraitPath) && string.IsNullOrWhiteSpace(card.PortraitPath) == false && ResourceLoader.Exists(card.PortraitPath))
                {
                    paths.Add(card.PortraitPath);
                }
            }

            // The loop above can only surface art that some living card currently points at, so
            // custom art whose card has been removed - or that has not been assigned yet - is
            // invisible to the roller. Scan the mod's own portrait folders directly as well.
            int beforeCustom = paths.Count;
            AddPortraitsUnder(CustomPortraitRoot, paths);
            Log.Info($"[PortraitSearchBox] {paths.Count - beforeCustom} custom portrait(s) added from disk.");
        }
        catch (Exception ex)
        {
            Log.Error($"[PortraitSearchBox] Failed to load portrait paths: {ex.Message}");
        }
        return paths;
    }

    /// <summary>
    /// Recursively collects usable <c>big/</c> portraits under <paramref name="directory"/>.
    /// Only the big variant is collected: it is what a card actually renders, and including the
    /// small siblings would fill the picker with same-named duplicates at the wrong resolution.
    /// </summary>
    private static void AddPortraitsUnder(string directory, List<string> paths)
    {
        using var dir = DirAccess.Open(directory);
        if (dir == null) return;

        dir.ListDirBegin();
        while (true)
        {
            string entry = dir.GetNext();
            if (string.IsNullOrEmpty(entry)) break;
            if (entry is "." or "..") continue;

            string full = $"{directory.TrimEnd('/')}/{entry}";

            if (dir.CurrentIsDir())
            {
                AddPortraitsUnder(full, paths);
                continue;
            }

            // Exported builds surface imported textures as .import / .remap siblings rather than
            // the source file, so strip that suffix before testing the resource path.
            if (full.EndsWith(".import", StringComparison.OrdinalIgnoreCase) ||
                full.EndsWith(".remap", StringComparison.OrdinalIgnoreCase))
            {
                full = full[..full.LastIndexOf('.')];
            }

            if (!full.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) continue;
            if (!full.Contains("/big/", StringComparison.OrdinalIgnoreCase)) continue;
            if (paths.Contains(full)) continue;
            if (!ResourceLoader.Exists(full)) continue;

            paths.Add(full);
        }
        dir.ListDirEnd();
    }
}