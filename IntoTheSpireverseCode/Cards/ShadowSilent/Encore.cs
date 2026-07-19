using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowIronclad;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class Encore() : ShadowSilentCard(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    private const string TriggeredKey = "HasTriggered";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new DynamicVar(TriggeredKey,0)
    ];
    
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [IntoTheSpireverseKeywords.Arcane];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Arcane)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
    
    public override async Task AfterHandEmptied(PlayerChoiceContext choiceContext, Player player)
    {
        if (!(player.PlayerCombatState is { Phase: PlayerTurnPhase.Play }) || player != Owner)
            return;

        if (Convert.ToBoolean(DynamicVars[TriggeredKey].BaseValue))
            return;

        DynamicVars[TriggeredKey].BaseValue = 1;
        
        await CardPileCmd.Add(this, PileType.Hand);
    }
    
    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;

        DynamicVars[TriggeredKey].BaseValue = 0;
        
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8m);
    }
}
