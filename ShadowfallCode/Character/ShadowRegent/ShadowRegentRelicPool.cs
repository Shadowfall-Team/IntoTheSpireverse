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
            ModelDb.Relic<CaptainsHat>(),
            //common
            ModelDb.Relic<FuzzyDice>(),
            //uncommon
            ModelDb.Relic<CommBadge>(),
            ModelDb.Relic<DilithiumCrystal>(),
            //rare
            ModelDb.Relic<ShipInABottle>(),
            ModelDb.Relic<Bobblehead>(),
            ModelDb.Relic<PurpleDough>(),
            //shop
            ModelDb.Relic<HulaFigure>(),
        ]);
    }
}