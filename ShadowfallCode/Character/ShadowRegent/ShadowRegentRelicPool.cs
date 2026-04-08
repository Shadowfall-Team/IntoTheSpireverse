using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Relics.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Character;

// TODO impl
public class ShadowRegentRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => ShadowRegent.CharacterId;
    public override Color LabOutlineColor => ShadowRegent.Color;

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return new List<RelicModel>([
            ModelDb.Relic<SpareBullet>(),
            ModelDb.Relic<ShadowFencingManual>(),
            ModelDb.Relic<ShadowGalacticDust>(),
            ModelDb.Relic<ShadowLunarPastry>(),
            ModelDb.Relic<ShadowMiniRegent>(),
            
        ]);
    }
}