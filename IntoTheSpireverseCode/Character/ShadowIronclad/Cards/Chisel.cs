using MegaCrit.Sts2.Core.Animation;
using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Chisel() : ShadowIroncladCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new ScryVar(1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/devoted_sculptor/devoted_sculptor_attack");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithAttackerAnim(CreatureAnimator.attackTrigger, Owner.Character.AttackAnimDelay + 0.25f)
            .WithHitFx(VfxCmd.slashPath)
            .Execute(choiceContext);

        await ScryCmd.Execute(choiceContext, this);

        // Deliberately not previewed. The top card of the Draw Pile is hidden information, and
        // CardCmd.Preview would flash it on screen, telling the player exactly what they are about
        // to draw. The upgrade still lands; they just find out when they draw it.
        var top = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault();
        if (top is { IsUpgradable: true })
            CardCmd.Upgrade(top);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Scry().UpgradeValueBy(1m);
    }
}
