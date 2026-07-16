using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards;

namespace IntoTheSpireverse.Cards;

public sealed class MultiCall() : ShadowDefectCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int times = ResolveEnergyXValue();

        if (times <= 0)
            return;

        var orbs = Owner.PlayerCombatState!.OrbQueue.Orbs.ToList();
        if (orbs.Count == 0)
            return;

        for (int i = 0; i < times; i++)
        {
            foreach (OrbModel orb in orbs)
            {
                await OrbCmd.Passive(choiceContext, orb, null);
                await Cmd.Wait(0.1f);
            }
        }
    }
}

