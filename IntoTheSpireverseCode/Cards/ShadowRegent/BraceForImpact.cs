using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.Entities.Players;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class BraceForImpact() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ShardsPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var result = await CardPileCmd.Add(this, CargoCardPile.CargoPileType);
        CardCmd.PreviewCardPileAdd(result);
    }

    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (Pile != null && Pile.Type == CargoCardPile.CargoPileType)
        {
            if (player == Owner)
            {
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}