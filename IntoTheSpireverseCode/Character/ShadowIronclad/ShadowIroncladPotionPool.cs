using BaseLib.Abstracts;
using Godot;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public class ShadowIroncladPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => ShadowIronclad.CharacterId;
    public override Color LabOutlineColor => ShadowIronclad.Color;
}