using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class LittleArrows() : ShadowSilentCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        int statusCount = PileType.Hand.GetPile(Owner).Cards.Count(c => c.Type == CardType.Status);

        if (statusCount > 0)
        {
            await DamageCmd
                .Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(statusCount)
                .FromCardCompatibility(this, cardPlay)
                .TargetingRandomOpponents(CombatState)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
