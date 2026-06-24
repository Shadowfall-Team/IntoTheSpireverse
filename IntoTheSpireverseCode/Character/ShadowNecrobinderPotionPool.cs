using BaseLib.Abstracts;
using Godot;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public class ShadowNecrobinderPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => ShadowNecrobinder.CharacterId;
    public override Color LabOutlineColor => ShadowNecrobinder.Color;
}
