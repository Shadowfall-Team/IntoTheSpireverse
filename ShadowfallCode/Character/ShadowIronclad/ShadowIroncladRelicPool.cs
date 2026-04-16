using BaseLib.Abstracts;
using Godot;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowIroncladRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => ShadowIronclad.CharacterId;
    public override Color LabOutlineColor => ShadowIronclad.Color;
}