using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Variables;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using IntoTheSpireverse.IntoTheSpireverseCode.Utils;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowRegent;

public class FuzzyDice : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        LoadAmmoHoverTip.FromLoadAmmo();
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new LoadAmmoVar(1)
    ];

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player) return;

        if (Owner.PlayerCombatState is { Energy: > 0 })
        {
            await LoadAmmoCmd.LoadAmmo(DynamicVars[LoadAmmoVar.Key].BaseValue, Owner);
        }
    }
}
