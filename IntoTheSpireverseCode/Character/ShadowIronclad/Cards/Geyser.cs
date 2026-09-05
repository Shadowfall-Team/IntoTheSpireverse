using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Statuses;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Geyser() : ShadowIroncladCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(21m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Mud>(false),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);

        var mud = (CardModel)CombatState.CreateCard<Mud>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(mud, PileType.Discard, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(7m);
}
