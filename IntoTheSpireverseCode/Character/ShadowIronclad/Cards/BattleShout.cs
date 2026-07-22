using BaseLib.Abstracts;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Modifications;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class BattleShout() : ShadowIroncladCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    private const string IncreaseKey = "Increase";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(IncreaseKey, 5m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Modify)
    ];

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attacks = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.Type == CardType.Attack && Modification.CanModify(c))
            .ToList();

        foreach (var card in attacks)
        {
            CardModifier.AddModifier<BattleShoutModification>(card, DynamicVars[IncreaseKey].IntValue);
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade() => DynamicVars[IncreaseKey].UpgradeValueBy(2m);
}
