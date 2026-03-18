using BaseLib.Abstracts;
using Godot;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowDefectRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => ShadowDefect.CharacterId;
    public override Color LabOutlineColor => ShadowDefect.Color;
}