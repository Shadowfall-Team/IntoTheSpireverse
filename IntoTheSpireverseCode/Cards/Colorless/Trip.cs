using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public class Trip() : IntoTheSpireverseCard(0,
    CardType.Skill,
    CardRarity.Token,
    TargetType.AnyEnemy,
    "regent")
{

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;
        if (IsUpgraded)
        {
            await PowerCmd.Apply<VulnerablePower>(
                new ThrowingPlayerChoiceContext(),
                CombatState.HittableEnemies,
                DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<VulnerablePower>(
                new ThrowingPlayerChoiceContext(),
                play.Target,
                DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        }
    }

    public override TargetType TargetType =>
        IsUpgraded ? TargetType.AllEnemies : TargetType.AnyEnemy;
}