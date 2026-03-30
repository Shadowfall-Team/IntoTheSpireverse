using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Shadowfall.ShadowfallCode.Powers.ShadowNecrobinder;

public class ZombificationPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side) return;
        var cards = PileType.Discard.GetPile(Owner.Player).Cards
            .Where(c => !c.Keywords.Contains(CardKeyword.Unplayable))
            .ToList()
            .UnstableShuffle(Owner.Player.RunState.Rng.CombatCardSelection)
            .Take(Amount);
        Flash();
        foreach (var card in cards)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }
}