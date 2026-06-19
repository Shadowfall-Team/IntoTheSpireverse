using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class BattleShout() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string IncreaseKey = "Increase";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(IncreaseKey, 3m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attacks = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.Type == CardType.Attack)
            .ToList();

        foreach (var card in attacks)
        {
            if (card.DynamicVars.Damage != null)
                card.DynamicVars.Damage.BaseValue += DynamicVars[IncreaseKey].BaseValue;
        }
    }

    protected override void OnUpgrade() => DynamicVars[IncreaseKey].UpgradeValueBy(2m);
}