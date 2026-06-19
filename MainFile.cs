using System.Reflection;
using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves.Runs;
using Shadowfall.ShadowfallCode.Cards;
using Shadowfall.ShadowfallCode.Config;

namespace Shadowfall;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Shadowfall"; //At the moment, this is used only for the Logger and harmony names.

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static readonly string CardsDirectory = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "ArtRoller");

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        Directory.CreateDirectory(CardsDirectory);

        CardArtRoller.RegisterAllFromDirectory(CardsDirectory);
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
        harmony.PatchAll();

        ModConfigRegistry.Register(ModId, new ShadowfallConfig());

        // SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(TheLaw));
    }
}