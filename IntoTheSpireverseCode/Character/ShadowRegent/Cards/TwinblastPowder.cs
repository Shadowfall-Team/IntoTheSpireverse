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
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("Shots", 2),
        new LoadAmmoVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Free shots: same resolution as the FIRE button, but no energy is required or spent.
        // Stops early once the stockpile runs dry.
        for (var i = 0; i < DynamicVars["Shots"].IntValue; i++)
        {
            if (!await AmmoResource.TryFireShot(Owner, chargeEnergy: false)) break;
        }

        if (IsUpgraded)
        {
            await LoadAmmoCmd.LoadAmmo(DynamicVars.LoadAmmo.BaseValue, Owner, this);
        }
    }
}
