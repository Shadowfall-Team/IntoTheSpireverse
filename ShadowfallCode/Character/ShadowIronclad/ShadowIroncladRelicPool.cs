using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowIroncladRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => ShadowIronclad.CharacterId;
    public override Color LabOutlineColor => ShadowIronclad.Color;
    
}