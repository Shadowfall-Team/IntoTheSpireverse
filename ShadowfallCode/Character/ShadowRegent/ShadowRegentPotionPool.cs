using BaseLib.Abstracts;
using Godot;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowRegentPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => ShadowRegent.CharacterId;
    public override Color LabOutlineColor => ShadowRegent.Color;
}