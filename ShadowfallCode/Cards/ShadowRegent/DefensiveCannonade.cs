using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Ammo;
using Shadowfall.ShadowfallCode.Commands;
using Shadowfall.ShadowfallCode.utils;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class DefensiveCannonade() : ShadowRegentCard(
    2,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("LoadAmmo", 2),
        new IntVar("Shots", 2),
        new BlockVar(6, ValueProp.Unpowered),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await LoadAmmoCmd.LoadAmmo(DynamicVars["LoadAmmo"].BaseValue, Owner, this);

        var power = await PowerCmd.Apply<DefensiveCannonadePower>(
            new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Block.BaseValue,
            Owner.Creature,
            this);
        power?.ShotsRemaining = DynamicVars["Shots"].IntValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["LoadAmmo"].UpgradeValueBy(1);
    }
}

public class DefensiveCannonadePower : CustomPowerModel, IHasSecondAmount, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private int _shotsRemaining;

    public int ShotsRemaining
    {
        get => _shotsRemaining;
        set
        {
            _shotsRemaining = value;
            InvokeDisplayAmountChanged();
        }
    }

    public string GetSecondAmount() => _shotsRemaining.ToString();

    public async void OnAmmoFired(Player player, IReadOnlyList<Creature> targets)
    {
        if (player.Creature != Owner) return;

        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, null);

        ShotsRemaining--;
        if (ShotsRemaining <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}