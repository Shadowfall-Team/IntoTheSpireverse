using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class IceBeam() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1),
        new PowerVar<TemporaryStrengthPower>(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        await PowerCmd.Apply<WeakPower>(play.Target, DynamicVars.Weak.BaseValue,
            Owner.Creature, this);

        await PowerCmd.Apply<TemporaryStrengthPower>(play.Target,
            DynamicVars[nameof(TemporaryStrengthPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(TemporaryStrengthPower)].UpgradeValueBy(1);
    }
}

public class IceBeamPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<IceBeam>();

    protected override bool IsPositive => false;
}