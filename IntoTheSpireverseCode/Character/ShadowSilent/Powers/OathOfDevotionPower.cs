using IntoTheSpireverse.IntoTheSpireverseCode.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class OathOfDevotionPower : ShadowPowerModel, IMaxMuddleCost
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle)
    ];

    public int ModifyMaxMuddleCost(CardModel card, int i)
    {
        return card.Owner.Creature == Owner ? i - Amount : i;
    }

    public Task AfterModifyingMaxMuddleCost(PlayerChoiceContext choiceContext, CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }
}