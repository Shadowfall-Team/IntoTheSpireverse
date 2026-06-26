using BaseLib.Abstracts;
using Godot;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent;

public class ShadowRegentPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => ShadowRegent.CharacterId;
    public override Color LabOutlineColor => ShadowRegent.Color;
}