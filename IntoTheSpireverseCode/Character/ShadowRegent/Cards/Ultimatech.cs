using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class Ultimatech() : ShadowRegentCard(
    0,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<UltimateStrike>(),
        HoverTipFactory.FromCard<UltimateDefend>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (CombatState == null) return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var selected = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, DynamicVars.Cards.IntValue),
            null,
            this);

        foreach (var card in selected.ToList())
        {
            // Basic rarity plus the tag is what every character's starter Strike/Defend has in
            // common, modded ones included; it also excludes Strike-tagged cards like Wild Strike.
            var isBasicStrike = card.Rarity == CardRarity.Basic && card.Tags.Contains(CardTag.Strike);
            var isBasicDefend = card.Rarity == CardRarity.Basic && card.Tags.Contains(CardTag.Defend);

            await CardCmd.Exhaust(choiceContext, card);

            if (!isBasicStrike && !isBasicDefend) continue;

            CardModel ultimate = isBasicStrike
                ? CombatState.CreateCard<UltimateStrike>(Owner)
                : CombatState.CreateCard<UltimateDefend>(Owner);

            await CardPileCmd.AddGeneratedCardToCombat(ultimate, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
