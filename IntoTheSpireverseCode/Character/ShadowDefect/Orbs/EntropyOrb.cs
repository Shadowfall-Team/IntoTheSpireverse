using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect.Orbs;

public class EntropyOrb : PlaceholderOrbModel
{
    protected override string PassiveSfx => "event:/sfx/characters/defect/defect_lightning_passive";
    protected override string EvokeSfx => "event:/sfx/characters/defect/defect_lightning_evoke";
    protected override string ChannelSfx => "event:/sfx/characters/defect/defect_lightning_channel";

    public override Color DarkenedColor => new Color("796606");

    public override Decimal PassiveVal => this.ModifyOrbValue(5M);
    public override Decimal EvokeVal => this.ModifyOrbValue(1M);

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        await Passive(choiceContext, null);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target != null) return;

        this.TriggerCompat();

        await Cmd.Wait(0.1f);
        await CreatureCmd.Damage(choiceContext, [Owner.Creature.CombatState!.HittableEnemies.First()],
            PassiveVal, ValueProp.Unpowered, Owner.Creature);
    }

    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(),
            target: Owner.Creature, amount: EvokeVal, applier: Owner.Creature, cardSource: null);

        Creature[] targets = [Owner.Creature];
        return targets;
    }
}