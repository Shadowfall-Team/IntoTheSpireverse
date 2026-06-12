using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Actions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Ammo;
using Shadowfall.ShadowfallCode.CardPiles;
using Shadowfall.ShadowfallCode.Cards.Colorless;

namespace Shadowfall.ShadowfallCode.ui;

public partial class NAmmoButton : NButton
{
    private static readonly string _scenePath = "res://Shadowfall/scenes/CaptainsShip.tscn";
    private static readonly string _megaLabelFont = "res://themes/kreon_bold_glyph_space_one.tres";

    private Player _player = null!;
    private bool _initialized;
    private bool _hasEverHadAmmo;
    private bool _isFiring;
    private CardPile? _pile;

    // Child nodes
    private Control _shipContainer = null!;
    private ShadowfallMegaRichTextLabel _damageLabel = null!;
    private ShadowfallMegaLabel _ammoCountLabel = null!;
    private ShadowfallMegaLabel _fireLabel = null!;
    private ShadowfallMegaLabel _energyCostLabel = null!;
    private TextureRect _energyIcon = null!;
    private TextureRect _damageIcon = null!;

    private Tween? _fadeTween;

    // Bob state
    private float _bobTime;
    private const float BobAmplitude = 5f;
    private const float BobFrequency = 2f;

    protected override string? ClickedSfx => "event:/sfx/ui/clicks/ui_click";
    protected override string? HoveredSfx => "event:/sfx/ui/clicks/ui_hover";

    public static NAmmoButton Create()
    {
        var button = ResourceLoader.Load<PackedScene>(_scenePath).Instantiate<NAmmoButton>();
        var font = PreloadManager.Cache.GetAsset<Font>(_megaLabelFont);
        ApplyFont(button.GetNode<ShadowfallMegaRichTextLabel>("%DamageLabel"), font,
            minSize: 22,
            maxSize: 28);
        ApplyFont(button.GetNode<ShadowfallMegaLabel>("%Count"), font, minSize: 32,
            maxSize: 32);
        ApplyFont(button.GetNode<ShadowfallMegaLabel>("%FireButtonLabel"),
            font, minSize: 20, maxSize: 20);
        ApplyFont(button.GetNode<ShadowfallMegaLabel>("%EnergyLabel"),
            font, minSize: 21, maxSize: 24);
        return button;
    }

    private static void ApplyFont(MegaLabel label, Font font, int minSize, int maxSize)
    {
        label.AddThemeFontOverride(ThemeConstants.Label.Font, font);
        label.MinFontSize = minSize;
        label.MaxFontSize = maxSize;
    }

    private static void ApplyFont(MegaRichTextLabel label, Font font, int minSize, int maxSize)
    {
        label.AddThemeFontOverride(ThemeConstants.RichTextLabel.NormalFont, font);
        label.MinFontSize = minSize;
        label.MaxFontSize = maxSize;
    }

    private static Texture2D GetAttackIntentTexture(int damage)
    {
        var tier = damage switch
        {
            < 5 => "1",
            < 10 => "2",
            < 20 => "3",
            < 40 => "4",
            _ => "5"
        };
        return PreloadManager.Cache.GetAsset<Texture2D>(ImageHelper.GetImagePath($"packed/intents/attack/intent_attack_{tier}.png"));
    }

    public override void _Ready()
    {
        _shipContainer = GetNode<Control>("ShipContainer");
        _damageLabel = GetNode<ShadowfallMegaRichTextLabel>("%DamageLabel");
        _ammoCountLabel = GetNode<ShadowfallMegaLabel>("%Count");
        _fireLabel = GetNode<ShadowfallMegaLabel>("%FireButtonLabel");
        _energyCostLabel = GetNode<ShadowfallMegaLabel>("%EnergyLabel");
        _energyIcon = GetNode<TextureRect>("%EnergyIcon");
        _damageIcon = GetNode<TextureRect>("%DamageIcon");

        ConnectSignals();

        Modulate = new Color(1, 1, 1, 0);
        Visible = false;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_pile != null)
            _pile.ContentsChanged -= OnPileContentsChanged;
        CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
    }

    public void Initialize(Player player)
    {
        _player = player;
        _pile = AmmoCardPile.AmmoPileType.GetPile(player);
        _pile.ContentsChanged += OnPileContentsChanged;
        _energyIcon.Texture = PreloadManager.Cache.GetAsset<Texture2D>(
            EnergyIconHelper.GetPath(_player.Character.CardPool));
        _initialized = true;
        UpdateState();
    }

    // -------------------------------------------------------------------------
    // Process (bob)
    // -------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        if (!_initialized) return;
        _bobTime += (float)delta * BobFrequency;
        _shipContainer.Position = new Vector2(
            _shipContainer.Position.X,
            Mathf.Sin(_bobTime) * BobAmplitude);
    }

    // -------------------------------------------------------------------------
    // NButton overrides
    // -------------------------------------------------------------------------

    protected override void OnFocus()
    {
        base.OnFocus(); // plays HoveredSfx
        UpdateFireLabel();
        var top = TopCard;
        if (top == null) return;
        NHoverTipSet.CreateAndShow(this,
                new[] { HoverTipFactory.FromCard(top) }.Concat(top.HoverTips))
            ?.SetAlignment(this, HoverTipAlignment.Left);
    }

    protected override void OnUnfocus()
    {
        UpdateFireLabel();
        NHoverTipSet.Remove(this);
    }

    protected override void OnPress()
    {
        base.OnPress(); // plays ClickedSfx
        UpdateFireLabel();
    }

    protected override void OnRelease()
    {
        if (!CanFire) return;

        _isFiring = true;
        UpdateState();

        var action = new PlayAmmoCardAction(_player);
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(action);
        WaitForActionComplete(action);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateFireLabel();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UpdateFireLabel();
    }

    // -------------------------------------------------------------------------
    // Event handlers
    // -------------------------------------------------------------------------

    private void OnPileContentsChanged()
    {
        if (!_hasEverHadAmmo && _pile!.Cards.Count > 0)
        {
            _hasEverHadAmmo = true;
            AnimIn();
        }

        UpdateState();
    }

    private void OnCombatStateChanged(CombatState state) => UpdateState();

    // -------------------------------------------------------------------------
    // State
    // -------------------------------------------------------------------------

    private CardModel? TopCard => _pile?.Cards.FirstOrDefault();

    private bool IsPlayerTurn =>
        NCombatRoom.Instance?.Ui.Hand.CurrentMode == NPlayerHand.Mode.Play;

    private bool CanFire
    {
        get
        {
            if (!_initialized || _isFiring || _player.PlayerCombatState == null)
                return false;
            var top = TopCard;
            if (top == null) return false;
            return _player.PlayerCombatState.Energy >= top.EnergyCost.GetWithModifiers(CostModifiers.All)
                   && IsPlayerTurn
                   && !CombatManager.Instance.IsOverOrEnding;
        }
    }

    private void UpdateState()
    {
        if (!_initialized) return;
        if (_player.PlayerCombatState == null) return;

        _ammoCountLabel.Text = _pile!.Cards.Count.ToString();

        var card = TopCard ?? ModelDb.Card<AmmoVolley>();
        var damage = (int)Hook.ModifyDamage(
            _player.RunState,
            _player.Creature.CombatState,
            null,
            _player.Creature,
            card.DynamicVars.CalculatedDamage.BaseValue,
            ValueProp.Move,
            card,
            ModifyDamageHookType.All,
            CardPreviewMode.Normal,
            out _);
        _damageLabel.Text = $"{damage}";
        _damageIcon.Texture = GetAttackIntentTexture(damage);
        _energyCostLabel.Text = card.EnergyCost.GetWithModifiers(CostModifiers.All).ToString();

        _shipContainer.Modulate = CanFire ? Colors.White : new Color(0.5f, 0.5f, 0.5f);
        SetEnabled(CanFire);
        UpdateFireLabel();
    }

    private void UpdateFireLabel()
    {
        if (!_isEnabled)
            _fireLabel.Modulate = StsColors.gray;
        else if (IsFocused)
            _fireLabel.Modulate = StsColors.red;
        else
            _fireLabel.Modulate = StsColors.cream;
    }

    // -------------------------------------------------------------------------
    // Async fire wait
    // -------------------------------------------------------------------------

    private async void WaitForActionComplete(PlayAmmoCardAction action)
    {
        while (action.State != GameActionState.Finished && action.State != GameActionState.Canceled)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        _isFiring = false;
        UpdateState();
    }

    // -------------------------------------------------------------------------
    // Animation
    // -------------------------------------------------------------------------

    private void AnimIn()
    {
        Visible = true;
        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "modulate:a", 1f, 0.3f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);
    }
}