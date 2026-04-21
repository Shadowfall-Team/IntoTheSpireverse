using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Temper() : ShadowIroncladCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    private const string RetaliationAmountKey = "RetaliationAmount";
    private const string StrengthAmountKey = "StrengthAmount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(RetaliationAmountKey, 2m),
        new DynamicVar(StrengthAmountKey, 1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RetaliationPower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<TemperRetaliationPower>(
            Owner.Creature, DynamicVars[RetaliationAmountKey].BaseValue,
            Owner.Creature, this);
        await PowerCmd.Apply<TemperStrengthPower>(
            Owner.Creature, DynamicVars[StrengthAmountKey].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[RetaliationAmountKey].UpgradeValueBy(1m);
        DynamicVars[StrengthAmountKey].UpgradeValueBy(1m);
    }
}