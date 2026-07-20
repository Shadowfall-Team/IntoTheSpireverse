using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder.Powers;

public class YourDoomIsNighPower : IntoTheSpireversePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card.Owner != Owner.Player) return;
        if (card.Type != CardType.Curse) return;
        Flash();
        var enemies = CombatState.HittableEnemies;
        if (enemies.Count == 0) return;
        var target = Owner.Player.RunState.Rng.CombatTargets.NextItem(enemies);
        if (target == null) return;
        await CreatureCmdCompatibility.Damage(choiceContext, target, Amount, ValueProp.Unblockable | ValueProp.Unpowered, Owner, null, null);
    }
}