using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using Shadowfall.ShadowfallCode.Powers;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public class BlackIce() : ShadowDefectCard(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BlackIcePower>(2),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromOrb<DarkOrb>(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<DarkOrb>(choiceContext, Owner);
        await PowerCmd.Apply<BlackIcePower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature,DynamicVars[nameof(BlackIcePower)].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BlackIcePower)].UpgradeValueBy(1m);
    }
}