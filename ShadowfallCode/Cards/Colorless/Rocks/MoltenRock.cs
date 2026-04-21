using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards.Colorless.Rocks;
using Shadowfall.ShadowfallCode.CardTags;
using Shadowfall.ShadowfallCode.Interfaces;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public sealed class MoltenRock() : RockCardBase(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    private const string IncreaseKey = "Increase";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar(IncreaseKey, 3m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_rock_shatter", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);

        decimal increase = DynamicVars[IncreaseKey].BaseValue;
        foreach (var card in Owner.PlayerCombatState.AllCards)
        {
            switch (card)
            {
                case IRockCard rock:
                    rock.BuffFromRockPlay(increase);
                    break;
                case GiantRock giant:
                    GiantRockBuffTracker.Add(giant, increase);
                    break;
            }
        }
    }

    protected override void OnUpgrade() => DynamicVars[IncreaseKey].UpgradeValueBy(2m);
}