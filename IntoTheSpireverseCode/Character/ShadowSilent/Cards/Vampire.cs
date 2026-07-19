using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class Vampire() : ShadowSilentCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),
        HoverTipFactory.FromPower<BleedPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await IntoTheSpireverseKeywords.ExecuteDevious(choiceContext, Owner, this, async () =>
        {
            if (cardPlay.Target.HasPower<BleedPower>())
            {
                var power = cardPlay.Target.GetPower<BleedPower>();
                if (power == null) return;
                await PowerCmd.Decrement(power);
                await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue, true);
            }
        });
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(1m);
    }
}
