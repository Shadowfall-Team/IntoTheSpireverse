using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class FirstOfficer() : ShadowRegentCard(0,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        IsUpgraded ? HoverTipFactory.FromCard<Coordinate>(true) : HoverTipFactory.FromCard<Coordinate>(),
        IsUpgraded ? HoverTipFactory.FromCard<BelieveInYou>(true) : HoverTipFactory.FromCard<BelieveInYou>(),
        IsUpgraded ? HoverTipFactory.FromCard<Lift>(true) : HoverTipFactory.FromCard<Lift>(),
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        if (play.Target != null)
        {
            var cardModel = CardFactory.GetDistinctForCombat(play.Target.Player,
                [
                    ModelDb.Card<Coordinate>(), 
                    ModelDb.Card<BelieveInYou>(),
                    ModelDb.Card<Lift>()
                ],
                1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();


            if (cardModel != null)
            {
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(cardModel);
                }

                await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
            }
        }
    }
}