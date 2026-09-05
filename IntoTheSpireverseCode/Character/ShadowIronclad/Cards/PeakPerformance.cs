using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class PeakPerformance() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const string ScryKey = "Scry";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(ScryKey, 3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Scry),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await ScryHelper.Scry(choiceContext, Owner, DynamicVars[ScryKey].IntValue);

        // Read after the Scry, so discarding the top cards changes which card is copied.
        var top = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault();
        if (top == null) return;

        await CardCmd.AutoPlay(choiceContext, top.CreateClone(), null);
    }

    protected override void OnUpgrade() => DynamicVars[ScryKey].UpgradeValueBy(3m);
}
