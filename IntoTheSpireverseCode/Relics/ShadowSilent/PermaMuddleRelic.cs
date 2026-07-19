﻿using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics;

public class PermaMuddleRelic : ShadowSilentRelic, IntoTheSpireverseKeywords.IShouldPermanentMuddleListener, IntoTheSpireverseKeywords.ICardMuddledListener
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle),
    ];
    
    public async Task AfterCardMuddled(ICombatState combatState, CardModel card)
    {
        if (card.Owner != Owner) 
            return;
        Flash();
    }

    public bool ShouldPermanentMuddle(CardModel card)
    {
        return card.Owner == Owner;
    }
}