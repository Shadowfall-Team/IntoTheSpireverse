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
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowIronclad;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics;

public class RetaliationScaleRelic : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    private const string retaliationKey = "Retaliation";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(retaliationKey, 3M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RetaliationPower>(),
    ];
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !cardPlay.Card.Tags.Contains(IntoTheSpireverseCardTags.Scale))
            return;
        Flash();
        await PowerCmd.Apply<RetaliationPower>(choiceContext, Owner.Creature, DynamicVars[retaliationKey].IntValue, Owner.Creature, null);
    }
}