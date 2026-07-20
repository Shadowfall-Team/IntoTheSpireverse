using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;

public class MuddleBlockRelic : ShadowSilentRelic, IntoTheSpireverseKeywords.ICardMuddledListener
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
    
    public async Task AfterCardMuddled(ICombatState combatState, CardModel card)
    {
        if (card.Owner != Owner) 
            return;
        Flash();
        
        await CreatureCmd.GainBlock(Owner.Creature, GetEffectiveCost(card, Owner), ValueProp.Unpowered, null);
    }
    
    private static int GetEffectiveCost(CardModel card, Player owner)
    {
        if (card.EnergyCost.CostsX)
            return owner.PlayerCombatState?.Energy ?? 0;
        return card.EnergyCost.GetWithModifiers(CostModifiers.All);
    }
}