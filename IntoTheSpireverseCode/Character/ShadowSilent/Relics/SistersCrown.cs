using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;

public class SistersCrown : ShadowSilentRelic, IOvercostListener, IBeforeEnergySpentListener, ICardGlowGoldListener, IModifyCardPlayResultLocation
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
    
    public async Task BeforeEnergySpent(CardModel card)
    {
        if (card.Owner.PlayerCombatState == null) return;
        if (card.Owner != Owner)
            return;
        EnergyBeforePlay = card.Owner.PlayerCombatState.Energy;
    }
    
    public bool ShouldCardGlowGold(CardModel card)
    {
        if (card.Owner != Owner || WasUsedThisCombat || card.Owner.PlayerCombatState == null)
            return false;
        if (card.EnergyCost.GetWithModifiers(CostModifiers.All) > card.Owner.PlayerCombatState.Energy)
        {
            return true;
        }
        return false;
    }
    
    public CardLocationCompatibility ModifyCardPlayResultLocationCompatibility(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocationCompatibility location)
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
        
        return new CardLocationCompatibility(card.Owner, PileType.Exhaust, CardPilePosition.Bottom);
    }
    
    public override Task AfterCombatEnd(CombatRoom _)
    {
        WasUsedThisCombat = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}

public record struct CardLocationCompatibility(Player Player, PileType PileType, CardPilePosition Position);