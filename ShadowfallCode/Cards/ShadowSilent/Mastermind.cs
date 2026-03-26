using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class Mastermind() : ShadowSilentCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Devious),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShadowfallKeywords.ExecuteDevious(choiceContext, Owner, this, async () =>
        {
            var candidates = PileType.Draw.GetPile(Owner).Cards
                .Concat(PileType.Discard.GetPile(Owner).Cards)
                .ToList();

            if (candidates.Count == 0)
                return;

            var selected = (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                candidates,
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();

            if (selected != null)
                await CardCmd.Exhaust(choiceContext, selected);
        });
    }

    protected override void OnUpgrade() {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
