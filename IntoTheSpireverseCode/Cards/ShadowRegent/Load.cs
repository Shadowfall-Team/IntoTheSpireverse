using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Variables;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using IntoTheSpireverse.IntoTheSpireverseCode.Utils;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Load() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new LoadAmmoVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await LoadAmmoCmd.LoadAmmo(DynamicVars[LoadAmmoVar.Key].BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}