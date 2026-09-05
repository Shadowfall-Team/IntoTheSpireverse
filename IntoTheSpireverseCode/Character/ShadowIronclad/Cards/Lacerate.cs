using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Lacerate() : ShadowIroncladCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const string HpLossKey = "HpLoss";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new RepeatVar(2),
        new DynamicVar(HpLossKey, 1m),
    ];
    
    private static readonly Color VfxTint = new Color("c01020");

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BloodbondPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damageResponse = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx(VfxCmd.slashPath)
            .Execute(choiceContext);

        var totalDamage = damageResponse.Results.Sum(r => r.Sum(dr => dr.TotalDamage));

        if (totalDamage > 0)
        {
            if (TestMode.IsOff)
            {
                var targetNode = NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target);
                if (targetNode != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
                        NGaseousImpactVfx.Create(targetNode.VfxSpawnPosition, VfxTint));
                }
            }

            await PowerCmd.Apply<BloodbondPower>(
                new ThrowingPlayerChoiceContext(),
                cardPlay.Target, (decimal)totalDamage,
                Owner.Creature, this);
        }

        await CreatureCmdCompatibility.Damage(choiceContext, Owner.Creature,
            DynamicVars[HpLossKey].BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}
