using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;


public sealed class Shellback() : ShadowSilentCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ShellbackPower>(2m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromCard<Scale>(),
            HoverTipFactory.FromKeyword(CardKeyword.Retain),
            HoverTipFactory.Static(StaticHoverTip.Block),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

        var power = await PowerCmd.Apply<ShellbackPower>(
            choiceContext, Owner.Creature,
            DynamicVars.Power<ShellbackPower>().BaseValue,
            Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Power<ShellbackPower>().UpgradeValueBy(2m);
    }
}