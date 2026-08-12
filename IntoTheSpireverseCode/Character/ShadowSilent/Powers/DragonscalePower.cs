using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class DragonscalePower : ShadowPowerModel
{
    private bool _isAddingScale;
    
    private bool IsAddingScale
    {
        get => _isAddingScale;
        set
        {
            AssertMutable();
            _isAddingScale = value;
        }
    }
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Scale>()
    ];
    
    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (Owner.Player == null || creator == null || creator.Creature != Applier || !(card is Scale) || IsAddingScale)
            return;
        IsAddingScale = true;
        Flash();
        var scales = Enumerable.Range(0, Amount)
            .Select(_ => CombatState.CreateCard<Scale>(Owner.Player));
        
        await CardPileCmd.AddGeneratedCardsToCombat(scales, PileType.Hand, Owner.Player);
        IsAddingScale = false;
    }
}