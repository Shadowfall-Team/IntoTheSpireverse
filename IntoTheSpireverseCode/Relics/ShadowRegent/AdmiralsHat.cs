using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowRegent;

public class AdmiralsHat : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Warp>()
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature.CombatState.RoundNumber > 1) return;

        var warp = player.Creature.CombatState.CreateCard<Warp>(player);
        await CardPileCmd.AddGeneratedCardToCombat(warp, PileType.Hand, player);
    }
}