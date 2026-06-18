using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
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
    private ComboControllerIcons _comboIcons = null!;
    private const int MaxPreviewCards = 3;
    private const float PreviewDefaultScale = 0.6f;
    private const float PreviewScaleDecrement = 0.15f;
    private const float PreviewYOffset = -15f;
    private const float PreviewXOffset = 140f;
    private const float PreviewSpacingX = -35f;
    private const float PreviewHoverShiftX = 25f;

    private Tween? _previewTween;

    private const float HideOffsetX = -150f;

    private const float TooltipOffsetY = -355f;

    protected override PileType Pile => CargoCardPile.CargoPileType;

    private static readonly string _scenePath = "res://Shadowfall/scenes/CargoPile.tscn";
    private static readonly string megaLabelFont = "res://themes/kreon_bold_glyph_space_one.tres";

    public static AddedNode<NCombatPilesContainer, NCargoPile> _ = new(container =>
    {
        var cargoPileButton = ResourceLoader.Load<PackedScene>(_scenePath).Instantiate<NCargoPile>();
        cargoPileButton.Name = "%CargoPile";
        cargoPileButton.Position = new Vector2(35, 700);

        var background = cargoPileButton.GetNode<TextureRect>("CountContainer/Background");
        background.Texture = ResourceLoader.Load<Texture2D>("res://images/packed/combat_ui/pile_button_count.png");

        var countLabel = cargoPileButton.GetNode<ShadowfallMegaLabel>("CountContainer/Count");
        var font = PreloadManager.Cache.GetAsset<Font>(megaLabelFont);
        countLabel.AddThemeFontOverride(ThemeConstants.Label.Font, font);
        countLabel.MinFontSize = 20;
        countLabel.MaxFontSize = 26;

        var addSymbol = cargoPileButton.GetNode<ShadowfallMegaLabel>("%AddSymbol");
        addSymbol.AddThemeFontOverride(ThemeConstants.Label.Font, font);
        addSymbol.MinFontSize = 20;
        addSymbol.MaxFontSize = 20;

        return cargoPileButton;
    });

    public override void _Ready()
    {
        ConnectSignals();
        _emptyPileMessage = new LocString("combat_messages", "OPEN_EMPTY_CARGO");
        _comboIcons = new ComboControllerIcons(
            GetNode<TextureRect>("%ControllerIcon2"), // LT
            GetNode<TextureRect>("%ControllerIcon"), // RT
            MegaInput.viewDrawPile,
            MegaInput.viewDiscardPile,
            GetNode<ShadowfallMegaLabel>("%AddSymbol"));

        Visible = false;
        SetAnimInOutPositions();
        Disable();
        _comboIcons.Refresh();
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        if (NControllerManager.Instance != null)
        {
            NControllerManager.Instance.ControllerDetected += OnControllerChanged;
            NControllerManager.Instance.MouseDetected += OnControllerChanged;
            NControllerManager.Instance.ControllerTypeChanged += OnControllerChanged;
        }

        if (NInputManager.Instance != null)
            NInputManager.Instance.InputRebound += OnControllerChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (NControllerManager.Instance != null)
        {
            NControllerManager.Instance.ControllerDetected -= OnControllerChanged;
            NControllerManager.Instance.MouseDetected -= OnControllerChanged;
            NControllerManager.Instance.ControllerTypeChanged -= OnControllerChanged;
        }

        if (NInputManager.Instance != null)
            NInputManager.Instance.InputRebound -= OnControllerChanged;
    }

    private void OnControllerChanged() => _comboIcons?.Refresh();

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
            NPreviewCardHolder.Create(cardNode, showHoverTips: isTop,
                scaleOnHover: false);
        if (holder == null) return null;


        AddChild(holder);
        MoveChild(holder, 0);
        holder.MouseFilter = Control.MouseFilterEnum.Pass;
        holder.FocusMode = Control.FocusModeEnum.None;
        holder.Hitbox.MouseFilter = Control.MouseFilterEnum.Pass;

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
        var hoverTip = new HoverTip(
            new LocString("static_hover_tips", "CARGO_PILE.title"),
            new LocString("static_hover_tips", "CARGO_PILE.description"));
        var tooltip = NHoverTipSet.CreateAndShow(this, hoverTip);
        if (tooltip != null)
        {
            var yOffset = _previewHolders.Count > 0 ? TooltipOffsetY : -220f;
            tooltip.GlobalPosition = GlobalPosition + new Vector2(0, yOffset);
        }

        _bumpTween?.Kill();
        _bumpTween = CreateTween();
        _bumpTween.TweenProperty(_icon, "scale", new Vector2(1.25f, 1.25f), 0.05);
        TweenPreviewCards(PreviewHoverShiftX);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        TweenPreviewCards(0f);
    }

    private void TweenPreviewCards(float targetShiftX)
    {
        if (_previewHolders.Count == 0) return;
        _previewTween?.Kill();
        _previewTween = CreateTween();
        _previewTween.SetParallel();
        for (var i = 0; i < _previewHolders.Count; i++)
        {
            var xOffset = PreviewXOffset + i * PreviewSpacingX + targetShiftX;
            var targetPos = GlobalPosition + new Vector2(xOffset, PreviewYOffset);
            _previewTween.TweenProperty(_previewHolders[i], "global_position", targetPos, 0.1);
        }
    }

    public override void AnimIn()
    {
        base.AnimIn();
        Visible = true;
    }
}