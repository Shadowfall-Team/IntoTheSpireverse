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

public class PoisonRelic : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    private const string multiplierKey = "Multiplier";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(multiplierKey, 25M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
    ];
    
    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource, 
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null)
            return 1m;
        if (dealer != Owner.Creature && !Owner.Creature.Pets.Contains(dealer))
            return 1m;
        if (target == null || !target.HasPower<PoisonPower>())
            return 1m;

        return 1m + DynamicVars[multiplierKey].BaseValue / 100m;
    }
}