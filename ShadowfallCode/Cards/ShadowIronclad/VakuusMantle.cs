using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Cards.Colorless;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class VakuusMantle() : ShadowIroncladCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VakuusMantlePower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<DonTheMantle>(IsUpgraded)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        if (IsUpgraded)
            await PowerCmd.Apply<VakuusMantleUpgradedPower>(
                Owner.Creature, DynamicVars["VakuusMantlePower"].BaseValue,
                Owner.Creature, this);
        else
            await PowerCmd.Apply<VakuusMantlePower>(
                Owner.Creature, DynamicVars["VakuusMantlePower"].BaseValue,
                Owner.Creature, this);
    }

    protected override void OnUpgrade() { }
}