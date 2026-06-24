using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

public sealed class SharpWit() : ShadowSilentCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new PowerVar<DrawCardsNextTurnPower>(2m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var discardPile = PileType.Discard.GetPile(Owner).Cards.ToList();

        if (discardPile.Count > 0)
        {
            var selected = await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                discardPile,
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue));

            foreach (var card in selected)
            {
                await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top, null, false);
            }
        }

        await PowerCmd.Apply<DrawCardsNextTurnPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars[nameof(DrawCardsNextTurnPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
