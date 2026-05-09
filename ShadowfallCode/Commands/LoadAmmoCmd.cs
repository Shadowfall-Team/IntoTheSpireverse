using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;
using Shadowfall.ShadowfallCode.Cards.Colorless;

namespace Shadowfall.ShadowfallCode.Commands;

public static class LoadAmmoCmd
{
    public static async Task<List<AmmoVolley>> LoadAmmo(decimal amount, Player player,
        AbstractModel? source)
    {
        List<AmmoVolley> enumerable;
        if (CombatManager.Instance.IsOverOrEnding)
        {
            enumerable = [];
        }
        else
        {
            var blades = GetVolleys(player, false).ToList();
            if (blades.Count == 0)
            {
                var sovereignBlade = player.Creature.CombatState.CreateCard<AmmoVolley>(player);
                // sovereignBlade.CreatedThroughForge = true;
                await CardPileCmd.AddGeneratedCardToCombat(sovereignBlade, PileType.Hand, player);
                blades.Add(sovereignBlade);
            }

            IncreaseVolleyRepeats(amount, player);
            // await Hook.AfterForge(player.Creature.CombatState, amount, player, source);
            enumerable = blades;
        }

        return enumerable;
    }

    private static void IncreaseVolleyRepeats(decimal amount, Player player)
    {
        var list = GetVolleys(player, true).ToList();
        foreach (var sovereignBlade in list)
        {
            sovereignBlade.AddRepeats(amount);
            sovereignBlade.AfterForged();
            // ForgeCmd.PlayCombatRoomForgeVfx(player, sovereignBlade);
        }

        PreviewSovereignBlade(list);
    }

    private static List<AmmoVolley> GetVolleys(Player player, bool includeExhausted)
    {
        return player.PlayerCombatState.AllCards.Where(delegate(CardModel c)
        {
            if (c.IsDupe)
            {
                return false;
            }

            if (!includeExhausted)
            {
                var pile = c.Pile;
                return pile == null || pile.Type != PileType.Exhaust;
            }

            return true;
        }).OfType<AmmoVolley>().ToList();
    }

    private static void PreviewSovereignBlade(List<AmmoVolley> blades)
    {
        if (TestMode.IsOn)
        {
            return;
        }

        if (!LocalContext.IsMine(blades.First()))
        {
            return;
        }

        var list = blades.Where(c => c.Pile is { Type: PileType.Hand }).ToList();
        foreach (var sovereignBlade in list)
        {
            var ncardSmithVfx = NCardSmithVfx.Create(NCombatRoom.Instance.Ui.Hand.GetCard(sovereignBlade), false);
            NRun.Instance?.GlobalUi.AboveTopBarVfxContainer.AddChildSafely(ncardSmithVfx);
        }

        var list2 = blades.Where(c => c.Pile is { Type: not PileType.Hand and not PileType.Exhaust }).ToList();
        if (list2.Count != 0)
        {
            var ncardSmithVfx2 = NCardSmithVfx.Create(list2, false);
            NRun.Instance.GlobalUi.CardPreviewContainer.AddChildSafely(ncardSmithVfx2);
        }
    }
}