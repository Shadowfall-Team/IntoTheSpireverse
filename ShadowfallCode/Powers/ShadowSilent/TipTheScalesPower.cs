using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public sealed class TipTheScalesPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Sly),
        HoverTipFactory.FromCard<Ward>(false),
    ];

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            foreach (var card in pile.GetPile(Owner.Player).Cards)
            {
                if (card is Ward)
                    CardCmd.ApplyKeyword(card, CardKeyword.Sly);
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card.Owner.Creature != Owner)
            return Task.CompletedTask;
        if (card is not Ward)
            return Task.CompletedTask;

        CardCmd.ApplyKeyword(card, CardKeyword.Sly);
        return Task.CompletedTask;
    }
}