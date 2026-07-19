using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class DefensiveCannonade() : ShadowRegentCard(
    2,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new LoadAmmoVar(2),
        new IntVar("Shots", 2),
        new BlockVar(7, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await LoadAmmoCmd.LoadAmmo(DynamicVars.LoadAmmo.BaseValue, Owner, this);

        var power = await PowerCmd.Apply<DefensiveCannonadePower>(
            new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Block.BaseValue,
            Owner.Creature,
            this);
        power?.ShotsRemaining = DynamicVars["Shots"].IntValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}