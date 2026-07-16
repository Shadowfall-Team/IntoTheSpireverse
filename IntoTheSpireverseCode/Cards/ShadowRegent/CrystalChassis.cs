using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class CrystalChassis() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("ExtraBlock", 2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo),
        HoverTipFactory.FromCard<UltimateDefend>(IsUpgraded)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;
        //TODO: currently this uses fastenpower, do you want this to be its own power with its own icon?
        await PowerCmd.Apply<FastenPower>(
            choiceContext,Owner.Creature,
            DynamicVars["ExtraBlock"].BaseValue,
            Owner.Creature,
            this);

        var ultimateDefend = CombatState.CreateCard<UltimateDefend>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(ultimateDefend);
        }

        var result = await CardPileCmd.AddGeneratedCardToCombat(ultimateDefend, CargoCardPile.CargoPileType, Owner);
        CardCmd.PreviewCardPileAdd(result);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExtraBlock"].UpgradeValueBy(1);
    }
}