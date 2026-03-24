using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class StarCharts() : ShadowRegentCard(
    0,
    CardType.Skill,
    CardRarity.Basic,
    TargetType.None)
{    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3, ValueProp.Move),
    ];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        // TODO: Put the top card of your draw pile in Cargo.
    }

    protected override void OnUpgrade()
    {

        DynamicVars.Block.UpgradeValueBy(3m);
    }
}