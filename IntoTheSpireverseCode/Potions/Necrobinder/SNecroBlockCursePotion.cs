using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Potions.Necrobinder;

[Pool(typeof(ShadowNecrobinderPotionPool))]
public class SNecroBlockCursePotion : IntoTheSpireversePotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Unpowered),
        new DynamicVar("TotalBlock", 5m),
    ];
    
    // TODO make it so when you are not in possession of the potion it still shows the correct values

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
    ];
    
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom) return Task.CompletedTask;
        UpdateTotalBlock();
        return Task.CompletedTask;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPile, AbstractModel? source)
    {
        if (card.Owner != Owner) return Task.CompletedTask;
        if (card.Type != CardType.Curse) return Task.CompletedTask;
        UpdateTotalBlock();
        return Task.CompletedTask;
    }
    
    public override Task AfterPotionProcured(PotionModel potion)
    {
        if (potion != this) return Task.CompletedTask;
        UpdateTotalBlock();
        return Task.CompletedTask;
    }

    private void UpdateTotalBlock()
    {
        int curseCount = PileType.Deck.GetPile(Owner).Cards.Count(c => c.Type == CardType.Curse);
        DynamicVars["TotalBlock"].BaseValue = DynamicVars.Block.BaseValue + (5m * curseCount);
    }

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        int curseCount = PileType.Deck.GetPile(Owner).Cards.Count(c => c.Type == CardType.Curse);
        decimal blockAmount = DynamicVars.Block.BaseValue + (5m * curseCount);
        await CreatureCmd.GainBlock(Owner.Creature, blockAmount, ValueProp.Unpowered, null);
    }
}