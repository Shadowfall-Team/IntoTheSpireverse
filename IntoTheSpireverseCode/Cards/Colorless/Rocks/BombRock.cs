using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless.Rocks;

[Pool(typeof(TokenCardPool))]
public sealed class BombRock() : RockCardBase(3, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust,
        IntoTheSpireverseKeywords.Linger,
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Linger),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(22m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_rock_shatter", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    // protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    // {
    //     int triggers = LingerHelper.GetTriggerCount(this);
    //     for (int i = 0; i < triggers; i++)
    //     {
    //         EnergyCost.AddThisCombat(-1);
    //         await LingerHelper.NotifyLingerTriggered(this, choiceContext);
    //     }
    // }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(8m);
}
