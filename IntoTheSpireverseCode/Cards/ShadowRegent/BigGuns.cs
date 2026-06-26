using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using IntoTheSpireverse.IntoTheSpireverseCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class BigGuns() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BigGunsPower>(2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<BigGunsPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars[nameof(BigGunsPower)].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BigGunsPower)].UpgradeValueBy(1);
    }
}