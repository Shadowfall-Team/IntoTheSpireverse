using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class PoweredPulse() : ShadowRegentCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("StrengthLoss", 5)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext,
        CardModel card, bool fromHandDraw)
    {
        if (card == this)
        {
            if (CombatState == null) return;

            foreach (var creature in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<PoweredPulsePower>(choiceContext, creature,
                    DynamicVars["StrengthLoss"].BaseValue * await GeneratePlayCount(CombatState, creature),
                    Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthLoss"].UpgradeValueBy(2);
    }
}