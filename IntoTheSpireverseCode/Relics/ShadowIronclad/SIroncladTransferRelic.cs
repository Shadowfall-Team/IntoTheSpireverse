using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowIronclad;

public class Bellows : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterDeath(
        PlayerChoiceContext choiceContext,
        Creature target,
        bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (target.Side == Owner.Creature.Side) return;

        var debuffs = target.Powers
            .Where(p => p.TypeForCurrentAmount == PowerType.Debuff)
            .Select(p => (PowerModel)p.ClonePreservingMutability())
            .ToList();

        if (debuffs.Count == 0) return;

        var livingEnemies = Owner.Creature.CombatState.HittableEnemies
            .Where(c => c != target)
            .ToList();

        if (livingEnemies.Count == 0) return;

        Flash();

        var recipient = Owner.RunState.Rng.CombatCardSelection.NextItem(livingEnemies);

        foreach (var debuff in debuffs)
        {
            var existingPower = recipient.GetPowerById(debuff.Id);

            if (existingPower != null && existingPower.InstanceType == PowerInstanceType.None) // Todo check this is the same
            {
                DoHackyThingsForSpecificPowers(existingPower);
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(),
                    existingPower, (decimal)debuff.Amount,
                    Owner.Creature, null);
            }
            else
            {
                var power = (PowerModel)debuff.ClonePreservingMutability();
                DoHackyThingsForSpecificPowers(power);
                await PowerCmd.Apply(new ThrowingPlayerChoiceContext(),
                    power, recipient, (decimal)debuff.Amount,
                    Owner.Creature, null);
            }
        }
    }

    private static void DoHackyThingsForSpecificPowers(PowerModel power)
    {
        if (power is ITemporaryPower temporaryPower)
            temporaryPower.IgnoreNextInstance();
    }
}
