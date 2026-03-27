using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.CardPiles;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class Plot() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(6, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        
        var discardPile = PileType.Discard.GetPile(Owner)
            .Cards.OrderBy(c => c.Rarity)
            .ThenBy(c => c.Id).ToList();
        
        var cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, discardPile, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();
        if (cardModel != null)
        {
            await CardPileCmd.Add(cardModel, CargoCardPile.CargoPileType);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}
