using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

public class ShadowFencingManual : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<AmmoPower>(1)
    ];

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext,
        CombatSide side)
    {
        if (side != CombatSide.Player) return;

        if (Owner.PlayerCombatState is { Energy: > 0 })
        {
            await PowerCmd.Apply<AmmoPower>(Owner.Creature,
                DynamicVars[nameof(AmmoPower)].BaseValue,
                Owner.Creature,
                null);
        }
    }
}