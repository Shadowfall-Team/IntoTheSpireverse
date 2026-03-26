using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class PoweredBeam() : ShadowRegentCard(1,
    CardType.Attack,
    CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<AmmoPower>(1)
    ];

    protected override void OnUpgrade()
    {
        //TODO: What should upgrade?
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext,
        CardModel card, bool fromHandDraw)
    {
        if (card == this)
        {
            await PowerCmd.Apply<ShardPower>(
                Owner.Creature, DynamicVars[nameof(AmmoPower)].BaseValue,
                Owner.Creature,
                this);
        }
    }
}