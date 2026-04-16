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
public sealed class Brace() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    
    // Renamed from "Parry"
    
    private const string BraceKey = "Brace";

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(BraceKey, 10m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BracePower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BracePower>(
            Owner.Creature, DynamicVars[BraceKey].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars[BraceKey].UpgradeValueBy(3m);
}