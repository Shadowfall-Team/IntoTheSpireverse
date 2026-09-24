using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

/// <summary>
/// The Replay is granted through ModifyCardPlayCount rather than written onto BaseReplayCount.
///
/// BaseReplayCount asserts the model is mutable, and ModelDb builds every canonical card as
/// immutable, so setting it from the constructor throws CanonicalModelException at startup. Doing
/// it later, once a mutable copy exists, would work for played cards but would leave the canonical
/// model - the one the compendium and the card library render - printing no Replay at all.
///
/// Going through the hook keeps the count off the model entirely. The cost is that the engine's
/// automatic "Replay N." line only reads BaseReplayCount, so the text and the hover tip are
/// declared here instead. In exchange the number carries an upgrade diff, which the engine's own
/// line does not.
/// </summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Lacerate() : ShadowIroncladCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const string HpLossKey = "HpLoss";
    private const string ReplayKey = "Replay";
    private const string TimesKey = "Times";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar(HpLossKey, 1m),
        new IntVar(ReplayKey, 2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.ReplayDynamic,
            new DynamicVar(TimesKey, DynamicVars[ReplayKey].BaseValue)),
    ];

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount) =>
        card == this ? playCount + DynamicVars[ReplayKey].IntValue : playCount;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx(VfxCmd.slashPath)
            .Execute(choiceContext);

        await CreatureCmdCompatibility.Damage(choiceContext, Owner.Creature,
            DynamicVars[HpLossKey].BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars[ReplayKey].UpgradeValueBy(1m);
}
