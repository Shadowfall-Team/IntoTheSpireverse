using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect.Cards;

public sealed class SludgeBomb : ShadowDefectCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(18m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Void>()
    ];

    protected override bool IsPlayable =>
        PileType.Exhaust.GetPile(Owner).Cards.Any(c => c is Void);

    public SludgeBomb()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var voidCard = PileType.Exhaust.GetPile(Owner).Cards
            .FirstOrDefault(c => c is Void);

        if (voidCard == null)
            return;

        await CardPileCmd.RemoveFromCombat(voidCard);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
