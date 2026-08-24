using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ui;

public partial class NShipDisplay : NButton
{
    private static readonly string _scenePath = IntoTheSpireverseResources.ShipDisplayScene;

    protected Player Player = null!;
    protected bool _initialized;
    private bool _hasEverHadAmmo;

    protected Control ShipContainer = null!;
    private ShaderMaterial? _hologramMaterial;
    protected IntoTheSpireverseMegaRichTextLabel DamageLabel = null!;
    protected TextureRect DamageIcon = null!;

    private Tween? _fadeTween;
    private Tween? _orbTween;

    private float _bobTime;
    private bool _orbOffsetApplied;
    private static readonly Vector2 OrbSlotOffset = new(90f, 160f);
    private const float BobAmplitude = 5f;
    private const float BobFrequency = 2f;

    protected override string? ClickedSfx => null;
    protected override string? HoveredSfx => "event:/sfx/ui/clicks/ui_hover";

    protected override bool AllowFocusWhileDisabled => true;

    public bool Initialized => _initialized;

    #region Godot Lifecycle

    public override void _Ready()
    {
        ShipContainer = GetNode<Control>("ShipContainer");
        _hologramMaterial = GetNode<TextureRect>("ShipContainer/ShipIcon").Material as ShaderMaterial;
        DamageLabel = GetNode<IntoTheSpireverseMegaRichTextLabel>("%DamageLabel");
        DamageIcon = GetNode<TextureRect>("%DamageIcon");

        ConnectSignals();

        Modulate = new Color(1, 1, 1, 0);
        Visible = false;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
        AmmoResource.AmmoChanged += OnAmmoChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
        AmmoResource.AmmoChanged -= OnAmmoChanged;
    }

    public override void _Process(double delta)
    {
        if (!_initialized) return;
        _bobTime += (float)delta * BobFrequency;
        var bobY = Mathf.Sin(_bobTime) * BobAmplitude;
        ShipContainer.Position = new Vector2(
            ShipContainer.Position.X,
            bobY);
        var containerHeight = ShipContainer.Size.Y;
        if (containerHeight > 0f)
            _hologramMaterial?.SetShaderParameter("uvOffsetY", bobY / containerHeight);
    }

    #endregion

    #region Initialization

    public static NShipDisplay Create()
    {
        var display = ResourceLoader.Load<PackedScene>(_scenePath).Instantiate<NShipDisplay>();
        ApplyDamageLabelFont(display);
        return display;
    }

    protected static void ApplyDamageLabelFont(NShipDisplay display)
    {
        var font = PreloadManager.Cache.GetAsset<Font>(IntoTheSpireverseResources.MegaLabelFont);
        var label = display.GetNode<IntoTheSpireverseMegaRichTextLabel>("%DamageLabel");
        label.AddThemeFontOverride(ThemeConstants.RichTextLabel.NormalFont, font);
        label.MinFontSize = 22;
        label.MaxFontSize = 28;
    }

    public virtual void Initialize(Player player)
    {
        Player = player;
        _initialized = true;
        SetEnabled(false);
        UpdateState();
    }

    #endregion

    #region Button Overrides

    protected override void OnFocus()
    {
        PlaySfx(HoveredSfx);
        DamageLabel.Visible = true;
        DamageIcon.Visible = true;
        ShipContainer.Modulate = Colors.White;
        SetHologramIntensity(opacity: 1, linesColorIntensity: 2);

        ShowHoverTip();
    }

    protected override void OnUnfocus()
    {
        DamageLabel.Visible = false;
        DamageIcon.Visible = false;
        ShipContainer.Modulate = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        SetHologramIntensity(opacity: 0.6f, linesColorIntensity: 1);

        HideHoverTip();
    }

    protected void SetHologramIntensity(float opacity, float linesColorIntensity)
    {
        _hologramMaterial?.SetShaderParameter("opacity", opacity);
        _hologramMaterial?.SetShaderParameter("linesColorIntensity", linesColorIntensity);
    }

    protected static void PlaySfx(string? sfx)
    {
        if (sfx != null) SfxCmd.Play(sfx);
    }

    protected void ShowHoverTip()
    {
        if (!_initialized) return;

        NHoverTipSet.CreateAndShow(this, LoadAmmoHoverTip.ForFireButton(Player))
            ?.SetAlignment(this, HoverTipAlignment.Right);
    }

    protected void HideHoverTip() => NHoverTipSet.Remove(this);

    #endregion

    #region Event Handlers

    private void OnAmmoChanged(PlayerCombatState pcs, int oldVal, int newVal)
    {
        if (!_initialized || pcs != Player.PlayerCombatState) return;
        if (!_hasEverHadAmmo && newVal > 0)
        {
            _hasEverHadAmmo = true;
            AnimIn();
        }

        UpdateState();
    }

    private void OnCombatStateChanged(CombatState state) => UpdateState();

    protected virtual void AnimIn()
    {
        Visible = true;

        ShipContainer.Modulate = new Color(0.5f, 0.5f, 0.5f);
        SetHologramIntensity(opacity: 0.6f, linesColorIntensity: 1);

        FadeIn();
    }

    protected void FadeIn()
    {
        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "modulate:a", 1f, 0.3f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);
    }

    #endregion

    #region State Updates

    protected virtual void UpdateState()
    {
        if (!_initialized) return;
        if (Player.PlayerCombatState == null) return;

        var damage = (int)AmmoResource.GetShotDamage(Player);
        DamageLabel.Text = $"{damage}";
        DamageIcon.Texture = GetAttackIntentTexture(damage);
    }

    protected static Texture2D GetAttackIntentTexture(int damage)
    {
        var tier = damage switch
        {
            < 5 => "1",
            < 10 => "2",
            < 20 => "3",
            < 40 => "4",
            _ => "5"
        };
        return PreloadManager.Cache.GetAsset<Texture2D>(
            ImageHelper.GetImagePath($"packed/intents/attack/intent_attack_{tier}.png"));
    }

    public void ApplyOrbOffset(bool hasOrbs)
    {
        if (hasOrbs && !_orbOffsetApplied)
        {
            _orbTween?.Kill();
            _orbTween = CreateTween();
            _orbTween.TweenProperty(this, "position", Position + OrbSlotOffset, 0.5f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
            _orbOffsetApplied = true;
        }
        else if (!hasOrbs && _orbOffsetApplied)
        {
            _orbTween?.Kill();
            _orbTween = CreateTween();
            _orbTween.TweenProperty(this, "position", Position - OrbSlotOffset, 0.5f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
            _orbOffsetApplied = false;
        }
    }

    #endregion
}
