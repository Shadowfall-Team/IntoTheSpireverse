using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class HitCountVarCompat
{
    // Using the original so it still gets any patches people happen to add to it
    static readonly MethodInfo? v113GetPreviewValue;
    static HitCountVarCompat()
    {
        v113GetPreviewValue = AccessTools.Method(AccessTools.TypeByName("MegaCrit.Sts2.Core.Localization.DynamicVars.HitCountVar"), nameof(GetPreviewValue));
    }
    public static decimal GetPreviewValue(CardModel card, int hits, bool runGlobalHooks)
    {
        if (v113GetPreviewValue is not null)
        {
            return (decimal)v113GetPreviewValue.Invoke(null, [card, hits, runGlobalHooks]);
        }

        return hits;
    }

    extension(DynamicVarSet varSet)
    {
        public SpireverseHitCountVar SpireverseHitCount => (SpireverseHitCountVar)varSet[SpireverseHitCountVar.DefaultName];
    }
}

public class SpireverseHitCountVar : DynamicVar
{
    public const string DefaultName = "HitCount";

    public SpireverseHitCountVar(int hits) : base(DefaultName, hits) { }
    public SpireverseHitCountVar(string name, int hits) : base(name, hits) { }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        PreviewValue = HitCountVarCompat.GetPreviewValue(card, IntValue, runGlobalHooks);
    }
}

public class SpireverseCalculatedHitCountVar(string name) : CalculatedVar(name)
{
    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        PreviewValue = HitCountVarCompat.GetPreviewValue(card, (int)Calculate(target), runGlobalHooks);
    }
}
