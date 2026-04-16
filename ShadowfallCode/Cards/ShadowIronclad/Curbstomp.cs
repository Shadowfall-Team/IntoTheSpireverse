using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Curbstomp() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string StrengthLossKey = "StrengthLoss";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(2m),
        new ShadowfallKeywords.GloryVar(4m),
        new DynamicVar(StrengthLossKey, 4m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ShadowfallKeywords.GloryHoverTipDynamic(DynamicVars[ShadowfallKeywords.GloryVar.defaultName]),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<WeakPower>(
            cardPlay.Target, DynamicVars.Weak.BaseValue,
            Owner.Creature, this);

        if (ShadowfallKeywords.IsGloryTriggered(this))
        {
            await PowerCmd.Apply<CurbstompPower>(
                cardPlay.Target, DynamicVars[StrengthLossKey].BaseValue,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Weak.UpgradeValueBy(1m);
}