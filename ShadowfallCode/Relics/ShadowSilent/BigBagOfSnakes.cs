using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Shadowfall.ShadowfallCode.Cards.Colorless;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Relics;

public class BigBagOfSnakes : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SoulRoll>(),
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Muddle),
    ];

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
            return;

        var combatState = player.Creature.CombatState;
        if (combatState == null)
            return;

        if (combatState.RoundNumber % 2 == 1)
        {
            Flash();
            var soulRoll = combatState.CreateCard<SoulRoll>(Owner);
            await CardPileCmd.AddGeneratedCardsToCombat(
                new[] { soulRoll }, PileType.Hand, Owner);
        }
        else
        {
            Flash();
            var drawn = await CardPileCmd.Draw(choiceContext, 1m, Owner);
            ShadowfallKeywords.ApplyMuddleAll(drawn);
        }
    }
}