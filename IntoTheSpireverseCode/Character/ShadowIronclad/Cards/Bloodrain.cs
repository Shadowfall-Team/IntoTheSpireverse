using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Bloodrain() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    private static readonly Color VfxTint = new Color("c01020");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(1m),
        new PowerVar<BloodbondPower>(8m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BloodbondPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // The trailing HP loss is deliberate: it triggers the Bloodbond this card just applied.
        await LoseHp(choiceContext, cardPlay);

        foreach (var enemy in CombatState.HittableEnemies.ToList())
        {
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

        await LoseHp(choiceContext, cardPlay);
    }

    private async Task LoseHp(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        await CreatureCmdCompatibility.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);

    protected override void OnUpgrade() => DynamicVars.Power<BloodbondPower>().UpgradeValueBy(3m);
}
