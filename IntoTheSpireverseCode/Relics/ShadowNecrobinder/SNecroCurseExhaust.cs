using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowNecrobinder;

public class SNecroCurseExhaust : ShadowNecrobinderRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
    ];
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        if (player.Creature.CombatState.RoundNumber != 1) return;

        var curses = PileType.Draw.GetPile(Owner).Cards
            .Where(c => c.Type == CardType.Curse)
            .ToList();

        if (curses.Count == 0) return;

        Flash();
        int toExhaust = curses.Count < 2 ? curses.Count : 2;
        for (int i = 0; i < toExhaust; i++)
        {
            var curse = Owner.RunState.Rng.CombatCardSelection.NextItem(curses);
            curses.Remove(curse);
            await CardCmd.Exhaust(choiceContext, curse);
        }
    }
}