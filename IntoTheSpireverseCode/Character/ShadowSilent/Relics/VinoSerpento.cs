using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;

public class VinoSerpento : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    private bool _wasUsedThisCombat;
    private bool _wasTriggered;
    
    private bool WasUsedThisCombat
    {
        get => _wasUsedThisCombat;
        set
        {
            AssertMutable();
            _wasUsedThisCombat = value;
        }
    }
    
    private bool WasTriggered
    {
        get => _wasTriggered;
        set
        {
            AssertMutable();
            _wasTriggered = value;
        }
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
    ];
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return Task.CompletedTask;
        WasUsedThisCombat = false;
        WasTriggered = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
    public override Task AfterCombatEnd(CombatRoom _)
    {
        WasTriggered = false;
        WasUsedThisCombat = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
    
    public override Decimal ModifyPowerAmountGivenAdditive(
        PowerModel power,
        Creature giver,
        Decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        return !(power is PoisonPower) || cardSource == null || giver != Owner.Creature || WasUsedThisCombat ? 0M : DynamicVars.Poison.BaseValue;
    }

    public override async Task AfterModifyingPowerAmountGiven(PowerModel power)
    {
        Flash();
        WasTriggered = true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        // Currently a hack to account for the main branch not having `cardPlay.Player` yet
        // will potentially break if a card like TheBall applies poison, 
        if (WasTriggered && cardPlay.Card.Owner == Owner && !WasUsedThisCombat)
        {
            WasUsedThisCombat = true;
            Status = RelicStatus.Normal;
        }
    }
}