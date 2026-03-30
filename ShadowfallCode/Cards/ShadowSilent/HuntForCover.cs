using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class HuntForCover() : ShadowSilentCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Ward>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(Owner).Cards.ToList();
        var selected = (await CardSelectCmd.FromHandForDiscard(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, minCount: 0, hand.Count),
            null,
            this)).ToList();

        foreach (var card in selected)
            await CardCmd.Discard(choiceContext, card);

        for (int i = 0; i < selected.Count; i++)
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Ward>(Owner), PileType.Hand, true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
