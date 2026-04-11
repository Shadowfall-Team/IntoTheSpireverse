using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.CardPiles;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class StarCharts() : ShadowRegentCard(
    0,
    CardType.Skill,
    CardRarity.Basic,
    TargetType.None)
//TODO: readd this code once it is handled within baselib    
//, ICustomTranscendenceTarget
{
    // public CardModel GetTranscendenceTransformedCard()
    // {
        // return ModelDb.Card<Constellation>();
    // }
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Cargo)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if (!Owner.Deck.IsEmpty)
        {
            var drawPile = PileType.Draw.GetPile(Owner);
            var card = drawPile.Cards.FirstOrDefault();
            if (card != null)
            {
                await CardPileCmd.Add(card, CargoCardPile.CargoPileType, skipVisuals: true);
                CardCmd.Preview(card);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }

}