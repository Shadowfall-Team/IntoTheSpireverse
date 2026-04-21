using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class Subterfuge() : ShadowSilentCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(15m, ValueProp.Move),
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Weight>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);

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
                await CardPileCmd.Add(card, PileType.Hand);
            }
        }

        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Weight>(Owner), PileType.Hand, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
