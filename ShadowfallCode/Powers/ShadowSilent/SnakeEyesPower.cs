using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public sealed class SnakeEyesPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner)
            return;

        var targets = PileType.Hand.GetPile(player).Cards
            .Where(ShadowfallKeywords.CanMuddle)
            .OrderByDescending(c => c.EnergyCost.GetWithModifiers(CostModifiers.All))
            .Take(Amount)
            .ToList();

        if (targets.Count == 0)
            return;

        Flash();
        ShadowfallKeywords.ApplyMuddleAll(targets);
    }
}