using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;

public class Mithridatium : ShadowSilentRelic, IShouldPermanentMuddleListener
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle),
    ];
    
    public bool ShouldPermanentMuddle(CardModel card)
    {
        return card.Owner == Owner;
    }

    public Task AfterShouldPermanentMuddle(PlayerChoiceContext choiceContext,CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }
}