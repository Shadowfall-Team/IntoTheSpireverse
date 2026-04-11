using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.CardPiles;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class ShadowCrystal() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ShardPower>(3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ShardPower>(),
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Cargo)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<ShardPower>(Owner.Creature,
            DynamicVars[nameof(ShardPower)].BaseValue, Owner.Creature, null);

        //TODO: check if the card preview to cargo pile works correctly
        await CardPileCmd.Add(this, CargoCardPile.CargoPileType);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(ShardPower)].UpgradeValueBy(1);
    }
}