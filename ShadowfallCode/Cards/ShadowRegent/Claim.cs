using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Commands;
using Shadowfall.ShadowfallCode.utils;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class Claim() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("LoadAmmo", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await LoadAmmoCmd.LoadAmmo(DynamicVars["LoadAmmo"].BaseValue, Owner, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}