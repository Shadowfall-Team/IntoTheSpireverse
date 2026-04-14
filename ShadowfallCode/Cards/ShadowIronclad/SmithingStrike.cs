using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class SmithingStrike() : ShadowIroncladCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
    ];
    
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (SmithingStrikePatch.LeftNeighbor != null)
            CardCmd.Upgrade(SmithingStrikePatch.LeftNeighbor);
        if (SmithingStrikePatch.RightNeighbor != null)
            CardCmd.Upgrade(SmithingStrikePatch.RightNeighbor);

        SmithingStrikePatch.LeftNeighbor = null;
        SmithingStrikePatch.RightNeighbor = null;
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}