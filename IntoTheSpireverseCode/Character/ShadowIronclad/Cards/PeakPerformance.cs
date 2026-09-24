using MegaCrit.Sts2.Core.Animation;
using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class PeakPerformance() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ScryVar(3m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await ScryCmd.Execute(choiceContext, this);

        // Read after the Scry, so discarding the top cards changes which card is copied.
        var top = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault();
        if (top == null) return;

        await CardCmd.AutoPlay(choiceContext, top.CreateClone(), null);
    }

    protected override void OnUpgrade() => DynamicVars.Scry().UpgradeValueBy(3m);
}
