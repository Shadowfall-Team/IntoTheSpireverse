using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class Siege() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new LoadAmmoVar(1),
        new PowerVar<SiegePower>(2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        LoadAmmoHoverTip.FromLoadAmmo()
            .Append(HoverTipFactory.FromPower<WeakPower>())
            .Append(HoverTipFactory.FromPower<VulnerablePower>());

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await LoadAmmoCmd.LoadAmmo(DynamicVars.LoadAmmo.BaseValue, Owner, this);

        await PowerCmd.Apply<SiegePower>(
            new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars[nameof(SiegePower)].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(SiegePower)].UpgradeValueBy(1);
    }
}