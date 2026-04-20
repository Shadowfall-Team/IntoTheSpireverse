using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowIroncladPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => ShadowIronclad.CharacterId;
    public override Color LabOutlineColor => ShadowIronclad.Color;
    
    
    protected override IEnumerable<PotionModel> GenerateAllPotions()
    {
        return
        [
            ModelDb.Potion<BloodPotion>(),
        ];
    }
    
}