using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;

namespace IntoTheSpireverse.IntoTheSpireverseCode.utils;

public static class AltCharacterUtil
{
    public static bool IsAvailableAltCharacter(CharacterModel c)
    {
        return c is IAltCharacter && (c is not IIntoTheSpireverseDebug);
    }
}
