using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Modifications;

/// <summary>
/// Applied by Shadow Crystal: the modified card also grants Block and a Shard when played.
/// </summary>
public sealed class ShadowCrystalModification : Modification
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ShardsPower>(1),
        new BlockVar(3m, ValueProp.Move)
    ];

    public override void AddTips(List<IHoverTip> tips)
    {
        base.AddTips(tips);
        tips.Add(HoverTipFactory.FromPower<ShardsPower>());
    }

    protected override ModelId SourceCardId => ModelDb.Card<ShadowCrystal>().Id;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner == null) return;

        var creature = Owner.Owner.Creature;

        await PowerCmd.Apply<ShardsPower>(new ThrowingPlayerChoiceContext(),
            creature,
            DynamicVars[nameof(ShardsPower)].BaseValue,
            creature,
            null);

        await CreatureCmd.GainBlock(creature, DynamicVars.Block, cardPlay);
    }
}
