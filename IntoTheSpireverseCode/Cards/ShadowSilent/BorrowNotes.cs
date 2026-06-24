using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

public sealed class BorrowedNotes() : ShadowSilentCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Weight>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var pools = Owner.UnlockState.CharacterCardPools.ToList();
        pools.Remove(Owner.Character.CardPool);

        var allCards = pools
            .SelectMany(p => p.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint));

        var rng = Owner.RunState.Rng.CombatCardGeneration;

        foreach (var type in new[] { CardType.Attack, CardType.Skill })
        {
            var card = CardFactory.GetDistinctForCombat(Owner, allCards.Where(c => c.Type == type), 1, rng).FirstOrDefault();
            if (card != null)
            {
                if (IsUpgraded)
                    CardCmd.Upgrade(card);

                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
            }
        }

        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Weight>(Owner), PileType.Hand, Owner);
    }
}
