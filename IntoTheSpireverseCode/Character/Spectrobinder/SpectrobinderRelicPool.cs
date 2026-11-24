using BaseLib.Abstracts;
using Godot;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.Spectrobinder;

public class SpectrobinderRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Spectrobinder.CharacterId;
    public override Color LabOutlineColor => Spectrobinder.Color;
}
