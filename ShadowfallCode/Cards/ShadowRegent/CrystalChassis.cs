using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.CardPiles;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class CrystalChassis() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("ExtraBlock", 3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Cargo),
        IsUpgraded ? HoverTipFactory.FromCard<UltimateDefend>(true) : HoverTipFactory.FromCard<UltimateDefend>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        //TODO: currently this uses fastenpower, do you want this to be its own power with its own icon?
        await PowerCmd.Apply<FastenPower>(Owner.Creature,
            DynamicVars["ExtraBlock"].BaseValue,
            Owner.Creature,
            this);

        var ultimateDefend = CombatState.CreateCard<UltimateDefend>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(ultimateDefend);
        }

        //TODO: check if the card preview to cargo pile works correctly
        var result = await CardPileCmd.Add(ultimateDefend, CargoCardPile.CargoPileType, source: this);
        CardCmd.PreviewCardPileAdd(result);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExtraBlock"].UpgradeValueBy(1);
    }
}