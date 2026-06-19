using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Config;

namespace Shadowfall.ShadowfallCode.utils;

public static class AltCharacterUtil
{
    public static bool IsAvailableAltCharacter(CharacterModel c)
    {
        return c is IAltCharacter && (c is not IShadowfallDebug || ShadowfallConfig.ShowWipContent);
    }
}
