using BaseLib.Abstracts;
using Godot;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent;

public class ShadowSilentPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => ShadowSilent.CharacterId;
    public override Color LabOutlineColor => ShadowSilent.Color;
}