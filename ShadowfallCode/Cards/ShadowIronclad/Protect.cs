using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Protect() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string ResolveKey = "Resolve";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(ResolveKey, 5m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ResolvePower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ResolvePower>(
            Owner.Creature, DynamicVars[ResolveKey].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars[ResolveKey].UpgradeValueBy(3m);
}