using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class VolatileCompound() : ShadowSilentCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await IntoTheSpireverseKeywords.ExecuteDevious(choiceContext, Owner, this, async() =>
        {
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
                if (hittableEnemy.HasPower<PoisonPower>())
                {
                    var power = hittableEnemy.GetPower<PoisonPower>();
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
        RemoveKeyword(CardKeyword.Exhaust);
}
