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
/// Replay goes through ModifyCardPlayCount because BaseReplayCount cannot be set on the immutable
/// canonical card, so the compendium would show no Replay. The engine's "Replay N." line reads only
/// BaseReplayCount, so the text and hover tip are declared here instead.
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
