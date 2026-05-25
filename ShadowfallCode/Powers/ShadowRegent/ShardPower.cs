using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Cards.Colorless;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Powers.ShadowRegent;

public class ShardPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Warp>()
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power != this) return;
        if (Owner.Player == null) return;

        if (Amount >= 6)
        {
            await AddWarpToHand();
        }
    }

    private async Task AddWarpToHand()
    {
        Flash();
        await Cmd.Wait(0.25f);

        var warp = CombatState.CreateCard<Warp>(Owner.Player);
        await CardPileCmd.AddGeneratedCardToCombat(warp, PileType.Hand, Owner.Player);

        SetAmount(Amount-6);
        if(Amount >= 6)
            await AddWarpToHand();
    }
}