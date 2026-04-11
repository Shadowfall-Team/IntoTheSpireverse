using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

[Pool(typeof(TokenCardPool))]
public class Fragment() : CustomCardModel(0,
    CardType.Skill,
    CardRarity.Token,
    TargetType.AllAllies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ShardPower>(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        if (CombatState == null) return;

        var players = CombatState.Players.Where(p => p.Creature.IsAlive);
        foreach (var player in players)
        {
            await PowerCmd.Apply<ShardPower>(player.Creature,
                DynamicVars[nameof(ShardPower)].BaseValue,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        //TODO: check what the upgrade to fragment should be
        DynamicVars[nameof(ShardPower)].UpgradeValueBy(1);
    }
}