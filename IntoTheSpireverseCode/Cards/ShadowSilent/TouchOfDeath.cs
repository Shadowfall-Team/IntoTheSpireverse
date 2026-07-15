using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class TouchOfDeath() : ShadowSilentCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<TouchOfDeathPower>(4m),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TouchOfDeathPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Power<TouchOfDeathPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<TouchOfDeathPower>().UpgradeValueBy(1m);
    }
}
