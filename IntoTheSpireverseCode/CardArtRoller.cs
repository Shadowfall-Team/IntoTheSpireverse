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
    public static string DefaultsDirectory { get; private set; } = "res://IntoTheSpireverse/ArtRoller";
    
    public static string DefaultsOutputDirectory { get; set; } = ""; // empty = use SaveDirectory/defaults

    private const string ConfigFileName = "card_art_roller_config.cfg";
    
    public static void RegisterAllFromDirectory(string directory)
    {
        SaveDirectory = directory;

        if (!Directory.Exists(directory))
        {
            GD.PrintErr($"[CardArtRoller] Directory not found: {directory}");
            return;
        }

        foreach (var path in Directory.GetFiles(directory, "*.hsv"))
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
        
        LoadConfig();
    }

    public static CardHsvData? GetCardData(string cardId) =>
        CardHsvModifiers.TryGetValue(cardId, out var data) ? data : null;

    public static CardHsvData? GetDefaultHsvForCard(string cardId)
    {
        // 1. Check user-overridable output directory first
        if (!string.IsNullOrEmpty(DefaultsOutputDirectory))
        {
            string userPath = Path.Combine(DefaultsOutputDirectory, $"{cardId}.hsv");
            if (File.Exists(userPath))
            {
                try
                {
                    return JsonSerializer.Deserialize<CardHsvData>(File.ReadAllText(userPath));
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"[CardArtRoller] Failed to load user default for '{cardId}': {ex.Message}");
                }
            }
        }

        // 2. Fall back to res:// packed defaults
        string resPath = $"{DefaultsDirectory}/{cardId}.hsv";
        if (!Godot.FileAccess.FileExists(resPath))
            return null;

        try
        {
            using var file = Godot.FileAccess.Open(resPath, Godot.FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"[CardArtRoller] Could not open '{resPath}': {Godot.FileAccess.GetOpenError()}");
                return null;
            }

            return JsonSerializer.Deserialize<CardHsvData>(file.GetAsText());
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CardArtRoller] Failed to load default data for '{cardId}': {ex.Message}");
            return null;
        }
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
        SaveToFile(data, GetDefaultOutputPath(cardId));
    }

    public static void SaveConfig()
    {
        try
        {
            var config = new { defaults_output_directory = DefaultsOutputDirectory };
            string path = GetConfigPath();
            string dir = Path.GetDirectoryName(path)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(path, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
            Log.Info($"[CardArtRoller] Config saved to {path}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CardArtRoller] Failed to save config: {ex.Message}");
        }
    }

    public static void LoadConfig()
    {
        try
        {
            string path = GetConfigPath();
            if (!File.Exists(path))
            {
                Log.Info("DOESNT EXIST: " + path);
                return;
            }

            Log.Info("EXIST: " + path);
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            if (doc.RootElement.TryGetProperty("defaults_output_directory", out var prop))
                DefaultsOutputDirectory = prop.GetString() ?? "";

            Log.Info($"[CardArtRoller] Config loaded. DefaultsOutputDirectory='{DefaultsOutputDirectory}'");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CardArtRoller] Failed to load config: {ex.Message}");
        }
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
        Path.Combine(ProjectSettings.GlobalizePath(SaveDirectory), $"{cardId}.hsv");

    private static string GetConfigPath() =>
        Path.Combine(ProjectSettings.GlobalizePath(SaveDirectory), ConfigFileName);

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
    
    private static string GetDefaultOutputPath(string cardId)
    {
        string dir = !string.IsNullOrEmpty(DefaultsOutputDirectory)
            ? DefaultsOutputDirectory
            : Path.Combine(ProjectSettings.GlobalizePath(SaveDirectory), "defaults");
        return Path.Combine(dir, $"{cardId}.hsv");
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