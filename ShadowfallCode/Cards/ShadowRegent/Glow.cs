using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class Glow() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        
        new CardsVar(1),
        new PowerVar<DrawCardsNextTurnPower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        await PowerCmd.Apply<ShardPower>(
            Owner.Creature,DynamicVars[nameof(ShardPower)].BaseValue, 
            Owner.Creature, 
            this);
        
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(Owner.Creature, DynamicVars["DrawCardsNextTurnPower"].BaseValue, Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        //TODO: upgrade not specified in the design doc
    }
}
