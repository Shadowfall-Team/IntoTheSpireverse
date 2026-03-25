using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class Bonecage() : ShadowNecrobinderCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string _lingerBlockKey = "LingerBlock";
    private Decimal _blockReduction;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9m, ValueProp.Move),
        new BlockVar(_lingerBlockKey, 5m, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Linger)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        ShadowfallKeywords.Linger
    ];
    
    private Decimal BlockReduction
    {
        get => _blockReduction;
        set
        {
            AssertMutable();
            _blockReduction = value;
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars[_lingerBlockKey].UpgradeValueBy(1m);
    }
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Block.BaseValue -= BlockReduction;
        DynamicVars[_lingerBlockKey].BaseValue -= BlockReduction;
    }
    
    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        int triggers = LingerHelper.GetTriggerCount(this);
        for (int i = 0; i < triggers; i++)
        {
            await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars[_lingerBlockKey], null);
            DynamicVars.Block.BaseValue -= 1m;
            DynamicVars[_lingerBlockKey].BaseValue -= 1m;
            BlockReduction += 1m;
        }
    }
    
}