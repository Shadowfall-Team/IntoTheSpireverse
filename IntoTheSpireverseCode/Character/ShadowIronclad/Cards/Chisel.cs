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
    // TODO: the patch notes also call for "Scry 1(2)". Scry does not exist anywhere in StS2
    // v0.111.0 - no card, keyword, hover tip or loc string - so it would have to be built as a
    // Spireverse keyword first. Damage and the top-card upgrade are implemented; Scry is not.
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/devoted_sculptor/devoted_sculptor_attack");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithAttackerAnim("Attack", Owner.Character.AttackAnimDelay + 0.25f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var top = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault();
        if (top is { IsUpgradable: true })
        {
            CardCmd.Upgrade(top);
            CardCmd.Preview(top);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}