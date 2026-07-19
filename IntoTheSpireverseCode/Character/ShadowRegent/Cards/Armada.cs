using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class Armada() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        if (IsUpgraded)
        {
            await LoadAmmoCmd.LoadAmmo(1, Owner, this);
        }

        await PowerCmd.Apply<ArmadaPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            1,
            Owner.Creature,
            this);
    }
}