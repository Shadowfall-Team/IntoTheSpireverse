using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.utils;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Grapeshot() : ShadowRegentCard(
    2,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<GrapeshotPower>(
            new ThrowingPlayerChoiceContext(), Owner.Creature,
            1,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

public class GrapeshotPower : ShadowPowerModel, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnAmmoFired(Player player, IEnumerable<List<DamageResult>> results)
    {
        if (player.Creature != Owner) return;
        Flash();

        var resultsList = results.ToList();
        for (var i = 0; i < Amount; i++)
        {
            foreach (List<DamageResult> result in resultsList)
            {
                IReadOnlyList<Creature>? hitTargets;
                if (Owner.HasPower<BigGunsPower>())
                {
                    hitTargets = CombatState.HittableEnemies;
                    await ShotHelper.CreateMissile(CombatState, null);
                }
                else
                {
                    hitTargets =
                        [Owner.Player.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies.ToList())];
                    await ShotHelper.CreateMissile(CombatState, hitTargets[0]);
                }

                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), hitTargets,
                    result.Max(d => d.TotalDamage) / 2M, ValueProp.Unpowered, Owner);
            }
        }
    }
}