using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Cards.Colorless.Rocks;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Interfaces;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;


[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Gravellize() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Common, TargetType.Self), IHandNeighborAware
{
    public CardModel? CapturedLeftNeighbor { get; set; }
    public CardModel? CapturedRightNeighbor { get; set; }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SpikedRock>(false),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        if (CapturedLeftNeighbor != null)
            await CardCmd.TransformTo<SpikedRock>(CapturedLeftNeighbor);
        if (CapturedRightNeighbor != null)
            await CardCmd.TransformTo<SpikedRock>(CapturedRightNeighbor);

        CapturedLeftNeighbor = null;
        CapturedRightNeighbor = null;
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
