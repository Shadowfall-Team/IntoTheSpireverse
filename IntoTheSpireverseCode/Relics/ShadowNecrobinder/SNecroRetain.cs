using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowNecrobinder;

public class SNecroRetain : ShadowNecrobinderRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain),
    ];

    public override Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return Task.CompletedTask;
        if (!Hook.ShouldFlush(player.Creature.CombatState, player)) return Task.CompletedTask;

        var hand = PileType.Hand.GetPile(Owner).Cards
            .Where(c => !c.ShouldRetainThisTurn)
            .ToList();

        if (hand.Count == 0) return Task.CompletedTask;

        Flash();
        var card = Owner.RunState.Rng.CombatCardSelection.NextItem(hand);
        card.GiveSingleTurnRetain();

        return Task.CompletedTask;
    }
}