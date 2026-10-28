using System.Reflection;
using BaseLib.Config;
using BaseLib.Utils;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent;
#if DEFECT
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect;
#endif
#if NECROBINDER
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder;
#endif
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using IntoTheSpireverse.IntoTheSpireverseCode.Metrics;
using IntoTheSpireverse.IntoTheSpireverseCode.Utils;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;
using Godot.Bridge;
using Godot;
using Logger = MegaCrit.Sts2.Core.Logging.Logger; // Godot also has a Logger class, but we want the MegaCrit one

namespace IntoTheSpireverse;

[ModInitializer(nameof(Initialize))]
public class MainFile
{
    public const string ModId = "IntoTheSpireverse"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
        harmony.PatchAll();

        ModConfigRegistry.Register(ModId, new IntoTheSpireverseConfig());
        MuddleListenerRegistry.Register("Snecko.SneckoCode.Core");

        CustomCharacterUtils.TryOrderCustomCharacters([
            typeof(ShadowIronclad),
            typeof(ShadowSilent),
            typeof(ShadowRegent),
#if NECROBINDER
            typeof(ShadowNecrobinder),
#endif
#if DEFECT
            typeof(ShadowDefect),
#endif
        ]);

        var customCursorPath = ProjectSettings.GetSetting("display/mouse_cursor/custom_image").AsString();
        var customHotspot = ProjectSettings.GetSetting("display/mouse_cursor/custom_image_hotspot").AsVector2();

        if (!string.IsNullOrEmpty(customCursorPath))
        {
            var cursorTexture = ResourceLoader.Load<Resource>(customCursorPath);
            Input.SetCustomMouseCursor(cursorTexture, Input.CursorShape.PointingHand, customHotspot);
        }

        ModManager.OnMetricsUpload += MetricsUploader.Handle;
    }
}
