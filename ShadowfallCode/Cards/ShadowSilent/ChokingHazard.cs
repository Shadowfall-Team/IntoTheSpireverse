using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class ChokingHazard() : ShadowSilentCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("StrengthLoss", 8m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromCard<Dazed>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PiercingWailPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, DynamicVars["StrengthLoss"].BaseValue, Owner.Creature, this);
        CardPileAddResult cardPileAddResult = await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Dazed>(Owner), PileType.Draw, Owner, CardPilePosition.Random);
        CardCmd.PreviewCardPileAdd(cardPileAddResult, 1.2f, CardPreviewStyle.HorizontalLayout);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthLoss"].UpgradeValueBy(2m);
    }
}
