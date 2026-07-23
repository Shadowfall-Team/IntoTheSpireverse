using BaseLib.Abstracts;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent;

public class ShadowSilentRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => ShadowSilent.CharacterId;
    public override Color LabOutlineColor => ShadowSilent.Color;
    
    
    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return new List<RelicModel>([
            //starter
            ModelDb.Relic<SneckoBrand>(),
            //common
            ModelDb.Relic<BaleteRoot>(),
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