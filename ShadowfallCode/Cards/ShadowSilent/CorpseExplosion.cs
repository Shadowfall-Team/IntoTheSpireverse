using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class CorpseExplosion() : ShadowSilentCard(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(6m),
        new PowerVar<CorpseExplosionPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await PowerCmd.Apply<PoisonPower>(
            choiceContext, cardPlay.Target,
            DynamicVars.Poison.BaseValue,
            Owner.Creature, this);

        await PowerCmd.Apply<CorpseExplosionPower>(
            choiceContext, cardPlay.Target,
            DynamicVars.Power<CorpseExplosionPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() =>
        DynamicVars.Poison.UpgradeValueBy(3m);
}