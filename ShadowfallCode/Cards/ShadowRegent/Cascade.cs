using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Shadowfall.ShadowfallCode.Ammo;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;
using Shadowfall.ShadowfallCode.utils;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

//TODO: has a name conflict with a vanilla card.
public class Cascade() : ShadowRegentCard(
    2,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<CascadePower>(
            new ThrowingPlayerChoiceContext(), Owner.Creature,
            1,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

public class CascadePower : CustomPowerModel, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async void OnAmmoFired(Player player, IReadOnlyList<Creature> targets)
    {
        if (player.Creature != Owner) return;
        Flash();
        await PowerCmd.Apply<VolleyDamagePower>(new ThrowingPlayerChoiceContext(), Owner, 1, Owner, null);
    }
}