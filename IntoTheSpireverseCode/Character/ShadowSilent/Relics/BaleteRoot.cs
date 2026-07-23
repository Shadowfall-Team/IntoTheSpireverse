using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;

public class BaleteRoot : ShadowSilentRelic, IntoTheSpireverseKeywords.IModifyDeviousListener
{
    public override RelicRarity Rarity => RelicRarity.Common;

    private const string increaseKey = "Increase";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(increaseKey, 1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),
    ];

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !cardPlay.Card.Keywords.Contains(IntoTheSpireverseKeywords.Devious))
            return Task.CompletedTask;
        Flash();
        return Task.CompletedTask;
    }

    public int ModifyDeviousValue(CardModel card, int originalValue)
    {
        return Owner != card.Owner ? originalValue : originalValue + DynamicVars[increaseKey].IntValue;
    }
}