using BaseLib.Abstracts;
using Godot;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowSilentPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => ShadowSilent.CharacterId;
    public override Color LabOutlineColor => ShadowSilent.Color;
}