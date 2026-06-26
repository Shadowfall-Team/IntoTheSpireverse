using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowIronclad;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowIronclad;

public sealed class Bloodrain() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Common, TargetType.RandomEnemy)
{
    private static readonly Color VfxTint = new("c01020");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BloodbondPower>(2m),
        new RepeatVar(3),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BloodbondPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        for (var i = 0; i < DynamicVars.Repeat.IntValue; i++)
        {
            var enemy = Owner.RunState.Rng.CombatTargets
                .NextItem(CombatState!.HittableEnemies);
            if (enemy == null)
            {
                continue;
            }

            if (TestMode.IsOff)
            {
                var targetNode = NCombatRoom.Instance?.GetCreatureNode(enemy);
                if (targetNode != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
                        NGaseousImpactVfx.Create(targetNode.VfxSpawnPosition, VfxTint));
                }
            }

            await PowerCmd.Apply<BloodbondPower>(
                new ThrowingPlayerChoiceContext(),
                enemy, DynamicVars.Power<BloodbondPower>().BaseValue,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Repeat.UpgradeValueBy(1m);
}