using MegaCrit.Sts2.Core.Animation;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;


public sealed class VolatileCompound() : ShadowSilentCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const string _deviousKey = "Devious";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(1m),
        new DynamicVar(_deviousKey, 0m),
    ];
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        IsUpgraded ? HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.DeviousX) : HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),

    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        await IntoTheSpireverseKeywords.ExecuteDevious(choiceContext, Owner, this, DynamicVars[_deviousKey].IntValue, async() =>
        {
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<VulnerablePower>(choiceContext, hittableEnemy, DynamicVars.Vulnerable.BaseValue,
                    cardPlay.Card.Owner.Creature, this);
                if (hittableEnemy.HasPower<PoisonPower>())
                {
                    var power = hittableEnemy.GetPower<PoisonPower>();
                    if (power == null) return;
                    await CreatureCmdCompatibility.Damage(new ThrowingPlayerChoiceContext(), power.Owner, power.Amount, ValueProp.Unblockable | ValueProp.Unpowered, this, cardPlay);
                    if (power.Owner.IsAlive)
                        await PowerCmd.Decrement(power);
                    else
                        await Cmd.CustomScaledWait(0.1f, 0.25f);
                }
            }
        });
    }

    protected override void OnUpgrade() =>
        DynamicVars[_deviousKey].UpgradeValueBy(1m);
}
