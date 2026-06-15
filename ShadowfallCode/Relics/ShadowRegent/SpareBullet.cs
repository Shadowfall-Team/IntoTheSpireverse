using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

public class SpareBullet : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature.CombatState.RoundNumber > 3) return;
        await PowerCmd.Apply<ShardPower>(
            new ThrowingPlayerChoiceContext(), Owner.Creature,
            2, Owner.Creature, null);
    }

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<Bandolier>();
    }
}
