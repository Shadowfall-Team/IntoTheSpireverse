using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class DarkBoost() : ShadowDefectCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Void>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);

        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
            CombatState.CreateCard<Void>(Owner),
            PileType.Draw,
            Owner, CardPilePosition.Top));

        await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}