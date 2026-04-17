using BaseLib.Config;

namespace Shadowfall.ShadowfallCode.Config;

[ConfigHoverTipsByDefault]
internal class ShadowfallConfig : SimpleModConfig
{
    [ConfigSection("ShadowRegent")]
    public static bool ShowCargoCardStack { get; set; } = true;
}