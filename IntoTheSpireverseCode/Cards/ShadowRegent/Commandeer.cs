using BaseLib.Extensions;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Logging;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Commandeer() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        
   
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        var pools = Owner.UnlockState.CharacterCardPools.ToList();
        pools.Remove(Owner.Character.CardPool);

        var allCards = pools
            .SelectMany(p => p.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint));

        var rng = Owner.RunState.Rng.CombatCardGeneration;

        foreach (var type in new[] { CardType.Attack})
        {
            var card = CardFactory.GetDistinctForCombat(Owner, allCards.Where(c => c.Type == type), 1, rng).FirstOrDefault();
            if (card != null)
            {
                if (IsUpgraded)
                {
                }
                else
                {
                    if (card.DynamicVars.TryGetValue("Damage", out var damage))
                    {
                        card.DynamicVars.Damage.BaseValue *= 2;
                    }
                    else if (card.DynamicVars.TryGetValue("CalculationBase", out var calculationBase))
                    {
                        card.DynamicVars.CalculatedDamage.BaseValue *= 2;
                    }
                }
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
            }
        }

    }
    
}
