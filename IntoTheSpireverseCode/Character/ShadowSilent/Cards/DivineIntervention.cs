using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class DivineIntervention() : ShadowSilentCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DivineInterventionPower>(1),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        IntoTheSpireverseKeywords.Devious,
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null)
            return;
        await IntoTheSpireverseKeywords.ExecuteDevious(choiceContext, Owner, this, async () =>
        {
            List<Creature> list = CombatState.GetTeammatesOf(Owner.Creature).Where(c => c.IsAlive && c.IsPlayer && c.Player != Owner).ToList();
            if (list.Count == 0)
                return;
            Creature? target = Owner.RunState.Rng.CombatTargets.NextItem(list)?.Player?.Creature;
            if (target != null)
            {
                if (target.HasPower<DivineInterventionPower>()) 
                    await PowerCmd.Apply<DivineInterventionPower>(choiceContext, target, DynamicVars["DivineInterventionPower"].BaseValue, Owner.Creature, this);
                else
                    await PowerCmd.Apply<DivineInterventionPower>(choiceContext, target, DynamicVars["DivineInterventionPower"].BaseValue + 1, Owner.Creature, this);
            }
        });
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
