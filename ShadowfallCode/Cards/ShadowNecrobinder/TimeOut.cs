using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Powers.ShadowNecrobinder;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class TimeOut() : ShadowNecrobinderCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string _strengthLossKey = "StrengthLoss";

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
        new CardsVar(1),
        new DynamicVar(_strengthLossKey, 4m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var drawn = await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        bool drewCurse = drawn.Any(c => c.Type == CardType.Curse);
        if (drewCurse)
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<TimeOutPower>(new ThrowingPlayerChoiceContext(), enemy, DynamicVars[_strengthLossKey].BaseValue, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}