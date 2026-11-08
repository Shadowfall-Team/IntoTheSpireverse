using BaseLib.Patches.Content;
using IntoTheSpireverse.IntoTheSpireverseCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode;

public static class IntoTheSpireverseHook
{
    extension(Hook)
    {
        public static bool ShouldDrawFromPile(ICombatState combatState, Player player, PileType pileType,
            bool startOfTurnDraw, out AbstractModel? preventer)
        {
            foreach (var model in Hook.IterateCombatHookListeners(combatState).OfType<IShouldDrawFromPileModifier>())
            {
                if (!model.ShouldDrawFromPile(pileType, player, startOfTurnDraw))
                {
                    MainFile.Logger.Debug($"Model {((AbstractModel)model).Id.Entry} prevented "
                        + $"{ player.Character.Id.Entry} from drawing from pile {CustomEnums.EnumName<PileType>((int)pileType)}");
                    preventer = model as AbstractModel;
                    return false;
                }
            }
            preventer = null;
            return true;
        }

        public static decimal ModifyStartOfTurnDrawFromPile(ICombatState combatState, Player player,
            PileType pileType, decimal originalCardCount, out Dictionary<AbstractModel, IEnumerable<PileType>> modifiers)
        {
            decimal currentCardCount = originalCardCount;
            Dictionary<AbstractModel, IEnumerable<PileType>> _modifiers = [];

            foreach (var modifier in Hook.IterateCombatHookListeners(combatState).OfType<IDrawFromPileModifier>())
            {
                var model = (AbstractModel)modifier;
                if (!_modifiers.ContainsKey(model)) { _modifiers[model] = []; }

                var lastCount = currentCardCount;
                currentCardCount = modifier.ModifyStartOfTurnDrawFromPile(pileType, player, currentCardCount);
                if (lastCount != currentCardCount)
                {
                    MainFile.Logger.Debug($"Model {model.Id.Entry} changed draw from pile "
                        + $"{CustomEnums.EnumName<PileType>((int)pileType)} from {lastCount} to {currentCardCount}");
                    _modifiers[model] = _modifiers[model].Append(pileType);
                }
            }

            modifiers = _modifiers;
            return currentCardCount;
        }

        public static async Task AfterModifyingStartOfTurnDrawFromPile(ICombatState combatState,
            Dictionary<AbstractModel, IEnumerable<PileType>> modifiers)
        {
            foreach (var modifier in Hook.IterateCombatHookListeners(combatState).OfType<IDrawFromPileModifier>())
            {
                var model = (AbstractModel)modifier;
                if (modifiers.ContainsKey(model))
                {
                    // call for each pile type the model modified
                    foreach (var pile in modifiers[model])
                    {
                        await modifier.AfterModifyingStartOfTurnDrawFromPile(pile);
                        model.InvokeExecutionFinished();
                    }
                }
            }
        }
    }
}
