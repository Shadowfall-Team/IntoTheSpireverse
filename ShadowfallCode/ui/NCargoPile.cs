using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using Shadowfall.ShadowfallCode.CardPiles;
using Shadowfall.ShadowfallCode.Config;

namespace Shadowfall.ShadowfallCode.ui;

public partial class NCargoPile : NCombatCardPile
{
    private List<NPreviewCardHolder> _previewHolders = [];
    private const int MaxPreviewCards = 3;
    private const float PreviewDefaultScale = 0.6f;
    private const float PreviewScaleDecrement = 0.15f;
    private const float PreviewYOffset = -15f;
    private const float PreviewXOffset = 140f;
    private const float PreviewSpacingX = -35f;

    private const float HideOffsetX = -150f;

    private const float TooltipOffsetY = -300f;

    protected override PileType Pile => CargoCardPile.CargoPileType;

    public override void _Ready()
    {
        ConnectSignals();
        _emptyPileMessage = new LocString("combat_messages", "OPEN_EMPTY_CARGO");

        _hoverTip = new HoverTip(new LocString("static_hover_tips", "CARGO_PILE.title"),
            new LocString("static_hover_tips", "CARGO_PILE.description"));

        Visible = false;
        SetAnimInOutPositions();
        Disable();
    }

    protected override void SetAnimInOutPositions()
    {
        _showPosition = Position;
        _hidePosition = Position + new Vector2(HideOffsetX, 0f);
    }

    public override void Initialize(Player player)
    {
        _localPlayer = player;
        _pile = Pile.GetPile(_localPlayer);
        _pile.ContentsChanged += HandleContentsChanged;

        _currentCount = _pile.Cards.Count;
        _countLabel.SetTextAutoSize(_currentCount.ToString());

        if (_pile.Cards.Count <= 0) return;
        Visible = true;
        Enable();
        CreateCardPreview();
    }

    private void HandleContentsChanged()
    {
        _currentCount = _pile.Cards.Count;
        _countLabel.SetTextAutoSize(_currentCount.ToString());

        if (_currentCount > 0 && Visible)
        {
            UpdateCardPreview();
        }
        else if (_currentCount > 0 && !Visible)
        {
            AnimIn();
            Enable();
            CreateCardPreview();
        }
        else if (_currentCount == 0 && Visible)
        {
            Disable();
            RemoveCardPreview();
        }
    }

    private void CreateCardPreview()
    {
        if (_pile.Cards.Count == 0 || !ShadowfallConfig.ShowCargoCardStack)
            return;

        var count = Math.Min(MaxPreviewCards, _pile.Cards.Count);

        for (var i = 0; i < count; i++)
        {
            var isTop = i == 0;
            var xOffset = PreviewXOffset + i * PreviewSpacingX;
            var scale = PreviewDefaultScale * (1f - i * PreviewScaleDecrement);
            var holder = CreatePreviewCard(_pile.Cards[i], isTop, xOffset, scale);
            if (holder != null)
            {
                _previewHolders.Add(holder);
            }
        }
    }

    private NPreviewCardHolder? CreatePreviewCard(CardModel card, bool isTop,
        float xOffset, float scale)
    {
        var cardNode = NCard.Create(card);
        if (cardNode == null) return null;

        var holder =
            NPreviewCardHolder.Create(cardNode, showHoverTips: false,
                scaleOnHover: false);
        if (holder == null) return null;


        AddChild(holder);
        MoveChild(holder, 0);
        holder.MouseFilter = MouseFilterEnum.Pass;
        holder.Hitbox.MouseFilter = MouseFilterEnum.Pass;

        PositionPreviewCard(holder, xOffset, scale);
        cardNode.UpdateVisuals(CargoCardPile.CargoPileType, CardPreviewMode.Normal);

        return holder;
    }

    private void PositionPreviewCard(NPreviewCardHolder holder, float xOffset,
        float scale)
    {
        holder.SetCardScale(new Vector2(scale, scale));
        holder.GlobalPosition = GlobalPosition + new Vector2(xOffset, PreviewYOffset);
    }

    private void UpdateCardPreview()
    {
        RemoveCardPreview();
        CreateCardPreview();
    }

    private void RemoveCardPreview()
    {
        foreach (var holder in _previewHolders)
        {
            holder.QueueFree();
        }

        _previewHolders.Clear();
    }

    protected override void OnFocus()
    {
        NHoverTipSet.Remove(this);
        var tooltip = NHoverTipSet.CreateAndShow(this, _hoverTip);
        tooltip.GlobalPosition = GlobalPosition + new Vector2(0, TooltipOffsetY);
        _bumpTween?.Kill();
        _bumpTween = CreateTween();
        _bumpTween.TweenProperty(_icon, "scale", new Vector2(1.25f, 1.25f), 0.05);
    }

    public override void AnimIn()
    {
        base.AnimIn();
        Visible = true;
    }
}