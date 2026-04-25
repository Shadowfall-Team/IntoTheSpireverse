using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Shadowfall.ShadowfallCode.Powers;

public sealed class CmbrPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<WhiteNoise>(),
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    };

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != base.Owner.Player)
        {
            return;
        }
        List<CardModel> cards = new List<CardModel>();
        for (int i = 0; i < base.Amount; i++)
        {
            CardModel card = combatState.CreateCard<WhiteNoise>(player);
            CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
            cards.Add(card);
        }
        Flash();
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, player);
    }
}