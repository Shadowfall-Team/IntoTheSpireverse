using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

public class CaptainsHat : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("Rounds", 1),
        new PowerVar<ShardPower>(2)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature.CombatState.RoundNumber > DynamicVars["Rounds"].IntValue) return;
        await PowerCmd.Apply<ShardPower>(
            new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Power<ShardPower>().BaseValue, Owner.Creature, null);
    }

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<AdmiralsHat>();
    }
}
