using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Relics;

public class HeartOfTheMountain : ShadowIroncladRelic
{
    private const string AbsorbKey = "Absorb";

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(AbsorbKey, 16)];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom) return;

        Flash();
        await PowerCmd.Apply<StoneHealthPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars[AbsorbKey].BaseValue,
            Owner.Creature, null);
    }
}
