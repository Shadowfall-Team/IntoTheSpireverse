using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.ammo;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Singletons;

public class ShellVolleySingleton() : CustomSingletonModel(true, false)
{
    public override Task BeforeCombatStart()
    {
        AmmoResource.OnAmmoFired += OnAmmoFired;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        AmmoResource.OnAmmoFired -= OnAmmoFired;
        return Task.CompletedTask;
    }

    private async void OnAmmoFired(Player player, IReadOnlyList<Creature> targets)
    {
        var creature = player.Creature;

        if (creature.HasPower<CascadePower>())
        {
            await PowerCmd.Apply<VolleyDamagePower>(new ThrowingPlayerChoiceContext(), creature, 1,
                creature, null);
        }

        if (creature.HasPower<SiegePower>())
        {
            foreach (var target in targets.Where(t => t.IsAlive))
            {
                await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, 1, creature, null);
            }
        }

        if (creature.HasPower<DefensiveCannonadePower>())
        {
            await CreatureCmd.GainBlock(creature, creature.GetPowerAmount<DefensiveCannonadePower>(),
                ValueProp.Move, null);
        }
    }
}
