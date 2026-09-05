using MegaCrit.Sts2.Core.Animation;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;


public sealed class Scalemail() : ShadowSilentCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new BlockVar(3m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Scale>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        var scales = Enumerable.Range(0, DynamicVars.Cards.IntValue)
            .Select(_ => CombatState.CreateCard<Scale>(Owner)); 
        
        await CardPileCmd.AddGeneratedCardsToCombat(scales, PileType.Hand, Owner);
        foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card is Scale)
            {
                await CreatureCmd.GainBlock(Owner.Creature,  DynamicVars.Block, cardPlay);
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
