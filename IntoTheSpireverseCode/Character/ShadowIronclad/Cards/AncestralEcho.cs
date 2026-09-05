using BaseLib.Abstracts;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Modifications;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class AncestralEcho() : ShadowIroncladCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const string CopiesKey = "Copies";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(CopiesKey, 1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Modify),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var top = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault();
        if (top == null || !Modification.CanModify(top)) return;

        CardModifier.AddModifier<AncestralEchoModification>(top, DynamicVars[CopiesKey].IntValue);
        CardCmd.Preview(top);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
