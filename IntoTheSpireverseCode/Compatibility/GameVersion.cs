using HarmonyLib;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public class GameVersion
{
    public static readonly bool HasCardLocation =
        AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardLocation") != null;
}