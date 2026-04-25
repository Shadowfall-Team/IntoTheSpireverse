using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class BlindFuryPower : CustomPowerModel
{
    private const int DrawCount = 3;
    private const int MaxCardsToPlay = 13;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override async Task AfterAutoPrePlayPhaseEnteredLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        Flash();

        await CardPileCmd.Draw(choiceContext, DrawCount, Owner.Player);

        bool hitLimit;
        using (CardSelectCmd.PushSelector(new VakuuCardSelector()))
        {
            int cardsPlayed = 0;
            while (cardsPlayed < MaxCardsToPlay &&
                   !CombatManager.Instance.IsOverOrEnding &&
                   !CombatManager.Instance.IsPlayerReadyToEndTurn(player))
            {
                var card = PileType.Hand.GetPile(Owner.Player).Cards
                    .FirstOrDefault(c => c.CanPlay());
                if (card == null)
                    break;

                var target = GetTarget(card, CombatState);
                await card.SpendResources();
                await CardCmd.AutoPlay(choiceContext, card, target, skipXCapture: true);
                cardsPlayed++;
            }

            hitLimit = cardsPlayed >= MaxCardsToPlay;

            if (cardsPlayed == 0)
            {
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -1, null, null);
                return;
            }
        }

        TalkCmd.Play(
            hitLimit
                ? new LocString("relics", "WHISPERING_EARRING.warning")
                : new LocString("relics", "WHISPERING_EARRING.approval"),
            Owner ,VfxColor.Purple);

        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -1, null, null);
    }

    private Creature? GetTarget(CardModel card, ICombatState combatState)
    {
        var rng = Owner.Player.RunState.Rng.CombatTargets;
        return card.TargetType switch
        {
            TargetType.AnyEnemy => combatState.HittableEnemies.FirstOrDefault(),
            TargetType.AnyPlayer => Owner,
            TargetType.AnyAlly => rng.NextItem(combatState.Allies
                .Where(c => c != null && c.IsAlive && c.IsPlayer && c != Owner)),
            _ => null
        };
    }
}