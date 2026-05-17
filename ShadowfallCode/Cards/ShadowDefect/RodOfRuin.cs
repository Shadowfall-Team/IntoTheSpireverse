using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Powers;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class RodOfRuin() : ShadowDefectCard(1, CardType.Skill, CardRarity.Basic, TargetType.None)
{
    public override bool GainsBlock => true;
 
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(4M, ValueProp.Move),
        new PowerVar<RodOfRuinPower>(2M),
    };
 
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromOrb<DarkOrb>()
    };
 
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
 
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
 
        await PowerCmd.Apply<RodOfRuinPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars["RodOfRuinPower"].BaseValue,
            Owner.Creature,
            this);
    }
 
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
    }
}