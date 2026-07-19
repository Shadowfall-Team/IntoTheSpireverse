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
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics;

public class OvercostRelic : ShadowSilentRelic, IOvercostListener, IBeforeEnergySpentListener
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    private bool _wasUsedThisCombat;
    
    private int _energyBeforePlay;
    
    private bool WasUsedThisCombat
    {
        get => _wasUsedThisCombat;
        set
        {
            AssertMutable();
            _wasUsedThisCombat = value;
        }
    }
    
    private int EnergyBeforePlay
    {
        get => _energyBeforePlay;
        set
        {
            AssertMutable();
            _energyBeforePlay = value;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
    ];

    public bool ShouldPlayAnyway(CardModel card)
    {
        if (card.Owner != Owner || WasUsedThisCombat)
            return false;
        return true;
    }
    
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return Task.CompletedTask;
        WasUsedThisCombat = false;
        EnergyBeforePlay = 0;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
    
    public Task BeforeEnergySpent(CardModel card)
    {
        if (card.Owner != Owner)
            return Task.CompletedTask;
        EnergyBeforePlay = card.Owner.PlayerCombatState.Energy;
        return Task.CompletedTask;
    }
    
    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation location)
    {
        if (card.Owner != Owner)
            return location;
        if (card.EnergyCost.GetResolved() <= EnergyBeforePlay)
            return location;
        if (isAutoPlay)
            return location;
        
        Flash();
        
        Status = RelicStatus.Normal;
        WasUsedThisCombat = true;
        EnergyBeforePlay = 0;
        location.pileType = PileType.Exhaust;
        
        return location;
    }
    
    public override Task AfterCombatEnd(CombatRoom _)
    {
        WasUsedThisCombat = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}