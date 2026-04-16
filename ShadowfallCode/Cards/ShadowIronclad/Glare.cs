using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Glare() : ShadowIroncladCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ShadowfallKeywords.GloryVar(1m),
        new PowerVar<VulnerablePower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ShadowfallKeywords.GloryHoverTipDynamic(DynamicVars[ShadowfallKeywords.GloryVar.defaultName]),
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    public override TargetType TargetType =>
        IsUpgraded ? TargetType.AllEnemies : TargetType.AnyEnemy;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!ShadowfallKeywords.IsGloryTriggered(this)) return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        if (IsUpgraded)
        {
            await PowerCmd.Apply<VulnerablePower>(
                (IEnumerable<Creature>)CombatState.HittableEnemies,
                DynamicVars.Vulnerable.BaseValue,
                Owner.Creature,
                this);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await PowerCmd.Apply<VulnerablePower>(
                cardPlay.Target,
                DynamicVars.Vulnerable.BaseValue,
                Owner.Creature,
                this);
        }
    }
    
    protected override void OnUpgrade() { }
}