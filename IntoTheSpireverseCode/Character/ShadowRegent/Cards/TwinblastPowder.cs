using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class TwinblastPowder() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const string ShotsKey = "Shots";
    private const string BonusDamageKey = "BonusDamage";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(ShotsKey, 2),
        new IntVar(BonusDamageKey, 4),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Free shots: same resolution as the FIRE button, but no energy is required or spent,
        // and each shot deals this card's bonus damage. The bonus is passed into the volley so
        // it lasts only for these shots rather than becoming a standing buff on the player.
        await FireAmmoCmd.FireVolley(
            DynamicVars[ShotsKey].IntValue, Owner, chargeEnergy: false,
            DynamicVars[BonusDamageKey].IntValue);
    }

    protected override void OnUpgrade() => DynamicVars[BonusDamageKey].UpgradeValueBy(2m);
}
