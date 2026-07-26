using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class TrialOfCombat() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // The power's Amount is the number of tokens it hands out, so upgrading the
        // card just raises the amount applied rather than needing a separate power.
        new PowerVar<TrialOfCombatPower>(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromEnchantment<Steady>(),
        SteadyMinionSacrificeTip()
    ];

    private static IHoverTip SteadyMinionSacrificeTip()
    {
        var preview = ModelDb.Card<MinionSacrifice>().ToMutable();
        CardCmd.Enchant<Steady>(preview, 1);
        return HoverTipFactory.FromCard(preview);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<TrialOfCombatPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(TrialOfCombatPower)].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(TrialOfCombatPower)].UpgradeValueBy(1);
    }
}
