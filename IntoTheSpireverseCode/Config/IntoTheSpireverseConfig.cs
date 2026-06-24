using BaseLib.Config;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Config;

[ConfigHoverTipsByDefault]
internal class IntoTheSpireverseConfig : SimpleModConfig
{
    [ConfigSection("ShadowRegent")]
    public static bool ShowCargoCardStack { get; set; } = true;

    /*
    [ConfigSection("Development")]
    public static bool ShowWipContent { get; set; } = false;
    */
}