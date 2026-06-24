using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Relics;
using IntoTheSpireverse.Orbs;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics;

  
public class CorruptedCore : ShadowDefectRelic
{

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            return (IEnumerable<DynamicVar>)[new DynamicVar("Entropy", 1M)];
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            return (IEnumerable<IHoverTip>) new IHoverTip[2]
            {
                HoverTipFactory.Static(StaticHoverTip.Channeling),
                HoverTipFactory.FromOrb<EntropyOrb>()
            };
        }
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        var corruptedCore = this;
        if (side != corruptedCore.Owner.Creature.Side || combatState.RoundNumber > 1)
            return;
        for (int i = 0; (Decimal) i < corruptedCore.DynamicVars["Entropy"].BaseValue; ++i)
            await OrbCmd.Channel<EntropyOrb>((PlayerChoiceContext) new BlockingPlayerChoiceContext(), corruptedCore.Owner);
    }
}