using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class SpotWeakness() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override bool ShouldGlowGoldInternal =>
        CombatState?.HittableEnemies.Any(e => e.Monster.IntendsToAttack) == true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        if (cardPlay.Target.Monster.IntendsToAttack)
        {
            await PowerCmd.Apply<StrengthPower>(
                Owner.Creature, DynamicVars["StrengthPower"].BaseValue,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["StrengthPower"].UpgradeValueBy(1m);
}