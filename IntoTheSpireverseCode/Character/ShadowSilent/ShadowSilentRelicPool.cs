using BaseLib.Abstracts;
using Godot;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent;

public class ShadowSilentRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => ShadowSilent.CharacterId;
    public override Color LabOutlineColor => ShadowSilent.Color;
    
    
    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return new List<RelicModel>([
            //starter
            ModelDb.Relic<MysticCharm>(),
            //common
            ModelDb.Relic<WiltedRose>(),
            //uncommon
            ModelDb.Relic<Gambeson>(),
            ModelDb.Relic<PrayerBeads>(),
            //rare
            ModelDb.Relic<ArmoredCore>(),
            ModelDb.Relic<SistersCrown>(),
            ModelDb.Relic<PaperSnecko>(),
            //shop
            ModelDb.Relic<Mithridatium>(),
        ]);
    }
}