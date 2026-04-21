using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Interfaces;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Chisel() : ShadowIroncladCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IHandNeighborAware
{
    public CardModel? CapturedLeftNeighbor { get; set; }
    public CardModel? CapturedRightNeighbor { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/devoted_sculptor/devoted_sculptor_attack");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithAttackerAnim("Attack", Owner.Character.AttackAnimDelay + 0.25f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (CapturedLeftNeighbor != null) CardCmd.Upgrade(CapturedLeftNeighbor);
        if (CapturedRightNeighbor != null) CardCmd.Upgrade(CapturedRightNeighbor);
        CapturedLeftNeighbor = null;
        CapturedRightNeighbor = null;
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
