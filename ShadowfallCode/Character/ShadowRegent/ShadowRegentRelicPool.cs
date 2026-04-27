using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Shadowfall.ShadowfallCode.Relics.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowRegentRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => ShadowRegent.CharacterId;
    public override Color LabOutlineColor => ShadowRegent.Color;

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return new List<RelicModel>([
            //starter
            ModelDb.Relic<SpareBullet>(),
            //common
            ModelDb.Relic<ShadowFencingManual>(),
            //uncommon
            ModelDb.Relic<Regalite>(),
            ModelDb.Relic<ShadowGalacticDust>(),
            //rare
            ModelDb.Relic<ShadowLunarPastry>(),
            ModelDb.Relic<ShadowMiniRegent>(),
            ModelDb.Relic<ShadowOrangeDough>(),
            //shop
            ModelDb.Relic<VitruvianMinion>(),
        ]);
    }
}