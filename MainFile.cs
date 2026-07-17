using System.Reflection;
using BaseLib.Config;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves.Runs;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;

namespace IntoTheSpireverse;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "IntoTheSpireverse"; //At the moment, this is used only for the Logger and harmony names.

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

        ModConfigRegistry.Register(ModId, new IntoTheSpireverseConfig());

        CustomCharacterUtils.TryOrderCustomCharacters([
            typeof(ShadowIronclad),
            typeof(ShadowRegent)
        ]);
    }
}