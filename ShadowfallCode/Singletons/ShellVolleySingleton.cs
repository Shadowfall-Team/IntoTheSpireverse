using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards.Colorless;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Singletons;

public class ShellVolleySingleton() : CustomSingletonModel(true, false)
{
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (cardSource == ModelDb.Card<AmmoVolley>() && dealer != null)
        {
            if (dealer.HasPower<CascadePower>())
            {
                await PowerCmd.Apply<VolleyDamagePower>(new ThrowingPlayerChoiceContext(), dealer, 1,
                    dealer, null);
            }

            if (dealer.HasPower<SiegePower>())
            {
                await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, 1, dealer, null);
            }

            if (dealer.HasPower<DefensiveCannonadePower>())
            {
                await CreatureCmd.GainBlock(dealer, dealer.GetPowerAmount<DefensiveCannonadePower>(),
                    ValueProp.Move, null);
            }
        }
    }
}