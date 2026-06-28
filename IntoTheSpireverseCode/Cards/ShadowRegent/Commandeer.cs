using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Commandeer() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var pools = Owner.UnlockState.CharacterCardPools.ToList();
        pools.Remove(Owner.Character.CardPool);

        var allCards = pools
            .SelectMany(p => p.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)).ToList();

        var rng = Owner.RunState.Rng.CombatCardGeneration;

        foreach (var type in new[] { CardType.Attack })
        {
            var card = CardFactory.GetDistinctForCombat(Owner, allCards.Where(c => c.Type == type), 1, rng)
                .FirstOrDefault();
            if (card == null) continue;
            var multiplier = IsUpgraded ? 3 : 2;
            if (card.DynamicVars.TryGetValue("Damage", out _))
            {
                card.DynamicVars.Damage.BaseValue *= multiplier;
            }
            else if (card.DynamicVars.TryGetValue("CalculationBase", out _))
            {
                card.DynamicVars.CalculatedDamage.BaseValue *= multiplier;
            }

            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }
}