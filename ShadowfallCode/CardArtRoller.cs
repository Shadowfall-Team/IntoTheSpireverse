using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using MegaCrit.Sts2.Core.Logging;

public class CardArtRoller
{
    public static Dictionary<string, CardHsvData> CardHsvModifiers { get; private set; } = new();
    public static string SaveDirectory = "user://card_hsv_data";

    public static void RegisterAllFromDirectory(string directory)
    {
        SaveDirectory = directory;

        if (!Directory.Exists(directory))
        {
            GD.PrintErr($"[CardArtRoller] Directory not found: {directory}");
            return;
        }

        foreach (var path in Directory.GetFiles(directory, "*.json"))
        {
            try
            {
                var hsvData = JsonSerializer.Deserialize<CardHsvData>(File.ReadAllText(path));
                if (hsvData != null && !string.IsNullOrEmpty(hsvData.CardId))
                {
                    CardHsvModifiers[hsvData.CardId] = hsvData;
                    Log.Info($"[CardArtRoller] Loaded data for: {hsvData.CardId}");
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[CardArtRoller] Failed to load '{path}': {ex.Message}");
            }
        }
    }

    public static CardHsvData? GetCardData(string cardId) =>
        CardHsvModifiers.TryGetValue(cardId, out var data) ? data : null;

    public static CardHsvData? GetDefaultHsvForCard(string cardId)
    {
        try
        {
            string path = Path.Combine(ProjectSettings.GlobalizePath(SaveDirectory), "defaults", $"{cardId}.json");
            if (File.Exists(path))
                return JsonSerializer.Deserialize<CardHsvData>(File.ReadAllText(path));
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CardArtRoller] Failed to load default data for '{cardId}': {ex.Message}");
        }
        return null;
    }

    public static void SaveHsvForCard(
        string cardId,
        float hue, float saturation, float value,
        float red = 100f, float green = 100f, float blue = 100f,
        float contrast = 100f,
        bool flipH = false,
        string portraitPath = "")
    {
        var data = BuildData(cardId, hue, saturation, value, red, green, blue, contrast, flipH, portraitPath);
        CardHsvModifiers[cardId] = data;
        SaveToFile(data, GetUserPath(cardId));
    }

    public static void DeleteHsvForCard(string cardId)
    {
        CardHsvModifiers.Remove(cardId);
        try
        {
            string path = GetUserPath(cardId);
            if (File.Exists(path))
            {
                File.Delete(path);
                Log.Info($"[CardArtRoller] Deleted data for {cardId}");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CardArtRoller] Failed to delete data for '{cardId}': {ex.Message}");
        }
    }

    public static void SaveDefaultHsvForCard(
        string cardId,
        float hue, float saturation, float value,
        float red = 100f, float green = 100f, float blue = 100f,
        float contrast = 100f,
        bool flipH = false,
        string portraitPath = "")
    {
        var data = BuildData(cardId, hue, saturation, value, red, green, blue, contrast, flipH, portraitPath);
        string defaultsDir = Path.Combine(ProjectSettings.GlobalizePath(SaveDirectory), "defaults");
        SaveToFile(data, Path.Combine(defaultsDir, $"{cardId}.json"));
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private static CardHsvData BuildData(
        string cardId,
        float hue, float saturation, float value,
        float red, float green, float blue,
        float contrast, bool flipH,
        string portraitPath) => new CardHsvData
    {
        CardId       = cardId,
        Hue          = hue        / 100f,
        Saturation   = saturation / 100f,
        Value        = value      / 100f,
        Red          = red        / 100f,
        Green        = green      / 100f,
        Blue         = blue       / 100f,
        Contrast     = contrast   / 100f,
        FlipH        = flipH,
        PortraitPath = string.IsNullOrWhiteSpace(portraitPath) ? null : portraitPath
    };

    private static string GetUserPath(string cardId) =>
        Path.Combine(ProjectSettings.GlobalizePath(SaveDirectory), $"{cardId}.json");

    private static void SaveToFile(CardHsvData data, string path)
    {
        try
        {
            string dir = Path.GetDirectoryName(path)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
            Log.Info($"[CardArtRoller] Saved data for {data.CardId} to {path}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CardArtRoller] Failed to save data for '{data.CardId}': {ex.Message}");
        }
    }
}

public class CardHsvData
{
    [JsonPropertyName("card_id")]       public string  CardId     { get; set; } = "";
    [JsonPropertyName("hue")]           public float   Hue        { get; set; } = 1f;
    [JsonPropertyName("saturation")]    public float   Saturation { get; set; } = 1f;
    [JsonPropertyName("value")]         public float   Value      { get; set; } = 1f;
    [JsonPropertyName("red")]           public float   Red        { get; set; } = 1f;
    [JsonPropertyName("green")]         public float   Green      { get; set; } = 1f;
    [JsonPropertyName("blue")]          public float   Blue       { get; set; } = 1f;
    [JsonPropertyName("contrast")]      public float   Contrast   { get; set; } = 1f;
    [JsonPropertyName("flip_h")]        public bool    FlipH      { get; set; } = false;
    [JsonPropertyName("portrait_path")] public string? PortraitPath { get; set; }
}