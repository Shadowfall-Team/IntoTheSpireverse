using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class SneckoForm() : ShadowSilentCard(4, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    private const string StatKey = "Stats";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(StatKey, 4m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ConfusedPower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

        await PowerCmd.Apply<ConfusedPower>(
            choiceContext, Owner.Creature, 1m,
            Owner.Creature, this);

        await PowerCmd.Apply<StrengthPower>(
            choiceContext, Owner.Creature, DynamicVars[StatKey].BaseValue,
            Owner.Creature, this);

        await PowerCmd.Apply<DexterityPower>(
            choiceContext, Owner.Creature, DynamicVars[StatKey].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() =>
        DynamicVars[StatKey].UpgradeValueBy(1m);
}