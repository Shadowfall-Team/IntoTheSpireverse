using Godot;
using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Logging;

namespace Shadowfall;

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
            string folder = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path)) ?? "";
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            string display = string.IsNullOrEmpty(folder) ? name : $"{folder}/{name}";
            
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
        }
        catch (Exception ex)
        {
            Log.Error($"[PortraitSearchBox] Failed to load portrait paths: {ex.Message}");
        }
        return paths;
    }
}