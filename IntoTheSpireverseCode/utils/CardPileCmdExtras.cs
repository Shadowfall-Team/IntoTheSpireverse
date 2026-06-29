using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.TestSupport;

namespace IntoTheSpireverse.IntoTheSpireverseCode.utils
{
    public static class CardPileCmdExtras
    {
        /// <summary>
        /// Moves a card from one pile to another and shows a preview in the center
        /// of the screen during the animation. Only use with 2 non-hand piles
        /// </summary>
        public static async Task TransferPileAndPreview(
            IEnumerable<CardModel> cards,
            PileType fromPileType,
            PileType toPileType,
            CardPreviewStyle style = CardPreviewStyle.HorizontalLayout
        )
        {
            if (TestMode.IsOn || CombatManager.Instance.IsEnding)
                return;

            float time = SaveManager.Instance.PrefsSave.FastMode >= FastModeType.Fast ? 0.6f : 1.2f;

            var results = (await CardPileCmd.Add(cards, toPileType, skipVisuals: true))
                .Where(r =>
                    r.success && LocalContext.IsMine(r.cardAdded) && r.cardAdded.Pile is not null
                )
                .ToList();

            if (results.Count == 0)
                return;

            var owner = results[0].cardAdded.Owner;

            if (!results.All(r => r.cardAdded.Owner == owner))
                return;

            if (style == CardPreviewStyle.HorizontalLayout && results.Count >= 5)
            {
                style = CardPreviewStyle.MessyLayout;
            }

            Control? container = style switch
            {
                CardPreviewStyle.HorizontalLayout => toPileType.IsCombatPile()
                    ? NCombatRoom.Instance?.Ui.CardPreviewContainer
                    : NRun.Instance?.GlobalUi.CardPreviewContainer,

                CardPreviewStyle.MessyLayout => toPileType.IsCombatPile()
                    ? NCombatRoom.Instance?.Ui.MessyCardPreviewContainer
                    : NRun.Instance?.GlobalUi.MessyCardPreviewContainer,

                CardPreviewStyle.EventLayout when !toPileType.IsCombatPile() => NRun.Instance
                    ?.GlobalUi
                    .EventCardPreviewContainer,

                CardPreviewStyle.GridLayout when !toPileType.IsCombatPile() => NRun.Instance
                    ?.GlobalUi
                    .GridCardPreviewContainer,

                CardPreviewStyle.EventLayout or CardPreviewStyle.GridLayout =>
                    throw new InvalidOperationException(),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(style),
                    $"Unexpected CardPreviewStyle {style}!"
                ),
            };

            if (container == null)
                return;

            var fromPile = fromPileType.GetPile(owner);

            List<NCard> nodes = [];
            foreach (var result in results)
            {
                fromPile?.InvokeCardRemoveFinished();
                NCard node = NCard.Create(result.cardAdded)!;
                node.Modulate = new Color(1, 1, 1, 0);
                container.AddChildSafely(node);
                node.UpdateVisuals(toPileType, CardPreviewMode.Normal);
                nodes.Add(node);
            }

            await container.ToSignal(container.GetTree(), "process_frame");

            foreach (var node in nodes)
            {
                Vector2 targetPos = node.GlobalPosition;
                node.Position = fromPileType.GetTargetPosition(node);
                node.Scale = Vector2.Zero;
                node.Modulate = Colors.White;

                Tween tween = node.CreateTween().SetParallel();
                tween
                    .TweenProperty(node, "position", targetPos, 0.4f)
                    .SetEase(Tween.EaseType.Out)
                    .SetTrans(Tween.TransitionType.Cubic);
                tween
                    .TweenProperty(node, "scale", Vector2.One, 0.4f)
                    .SetEase(Tween.EaseType.Out)
                    .SetTrans(Tween.TransitionType.Cubic);

                tween.Chain().TweenInterval(time);
                tween
                    .Chain()
                    .TweenCallback(
                        Callable.From(() =>
                        {
                            Node? vfxContainer =
                                toPileType != PileType.Deck
                                    ? node.Model?.Owner.Creature.GetVfxContainer()
                                    : NRun.Instance?.GlobalUi.TopBar.TrailContainer;

                            string? trailPath = node.Model?.Owner.Character.TrailPath;

                            if (vfxContainer is null || trailPath is null)
                            {
                                node.QueueFreeSafely();
                                return;
                            }

                            var flyVfx = NCardFlyVfx.Create(node, toPileType, true, trailPath);

                            if (flyVfx is null)
                            {
                                node.QueueFreeSafely();
                                return;
                            }

                            vfxContainer.AddChildSafely(flyVfx);
                        })
                    );
            }
        }
    }
}
