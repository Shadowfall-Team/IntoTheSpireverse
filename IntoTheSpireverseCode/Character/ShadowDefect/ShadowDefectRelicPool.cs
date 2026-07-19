using BaseLib.Abstracts;
using Godot;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect;

public class ShadowDefectRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => ShadowDefect.CharacterId;
    public override Color LabOutlineColor => ShadowDefect.Color;
}