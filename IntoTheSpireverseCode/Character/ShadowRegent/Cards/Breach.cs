using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class Breach() : ShadowRegentCard(2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(18, ValueProp.Move),
        new IntVar("Increase", 2)
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);

        await PowerCmd.Apply<BreachPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars["Increase"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
    }
}