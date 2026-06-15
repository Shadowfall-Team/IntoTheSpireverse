using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Cards.Colorless;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

public class Bandolier : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Warp>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("FreeShots", 3)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature.CombatState.RoundNumber > 1) return;

        var warp = player.Creature.CombatState.CreateCard<Warp>(player);
        await CardPileCmd.AddGeneratedCardToCombat(warp, PileType.Hand, player);
    }
}