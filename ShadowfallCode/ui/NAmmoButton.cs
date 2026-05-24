using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Actions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Shadowfall.ShadowfallCode.Ammo;

namespace Shadowfall.ShadowfallCode.ui;

public partial class NAmmoButton : Node2D
{
    // TODO: uncomment when .tscn is ready
    // private static readonly string _scenePath = "res://Shadowfall/scenes/AmmoButton.tscn";

    private Player _player = null!;
    private bool _initialized;
    private bool _hasEverHadAmmo;
    private bool _isFiring;

    // Child nodes
    private Node2D _shipNode = null!;
    private Label _damageLabel = null!;
    private Label _ammoCountLabel = null!;
    private Button _hitbox = null!;
    private ColorRect _fireRect = null!;
    private Label _fireLabel = null!;
    private Label _energyCostLabel = null!;

    // TODO: replace with real nodes from scene
    // private TextureRect _damageIcon = null!;
    // private Control _buttonBody = null!;
    // private TextureRect _energyOrb = null!;
    // private ShadowfallMegaLabel _energyCostLabel = null!;

    private Tween? _fadeTween;

    // Bob state
    private float _bobTime;
    private const float BobAmplitude = 3f;
    private const float BobFrequency = 2f;

    // TODO: replace with scene instantiation
    // public static NAmmoButton Create()
    // {
    //     return ResourceLoader.Load<PackedScene>(_scenePath).Instantiate<NAmmoButton>();
    // }

    public static NAmmoButton Create() => new();

    /// <summary>
    /// Called by NCreaturePatch. Stores the player so Initialize can run once _Ready fires.
    /// </summary>
    public void SetPlayer(Player player)
    {
        _pendingPlayer = player;
    }

    private Player? _pendingPlayer;

    public override void _Ready()
    {
        // TODO: replace this entire method body with GetNode lookups once .tscn exists:
        // _shipNode        = GetNode<Node2D>("%Ship");
        // _damageIcon      = GetNode<TextureRect>("%DamageIcon");
        // _damageLabel     = GetNode<ShadowfallMegaLabel>("%DamageLabel");
        // _buttonBody      = GetNode<Control>("%ButtonBody");
        // _ammoCountLabel  = GetNode<ShadowfallMegaLabel>("%AmmoCount");
        // _hitbox          = GetNode<Button>("%Hitbox");
        // _energyOrb       = GetNode<TextureRect>("%EnergyOrb");
        // _energyCostLabel = GetNode<ShadowfallMegaLabel>("%EnergyCost");
        // _fireLabel       = GetNode<ShadowfallMegaLabel>("%FireLabel");
        // _hitbox.Pressed += OnButtonPressed;

        BuildPlaceholderUi();

        Modulate = new Color(1, 1, 1, 0);
        Visible = false;

        if (_pendingPlayer != null)
            Initialize(_pendingPlayer);
    }

    /// <summary>
    /// Temporary code-built UI for testing. Remove entirely when .tscn is wired up.
    /// </summary>
    private void BuildPlaceholderUi()
    {
        // Ship node — bobs in _Process. Contains ship rect + damage label + hitbox.
        _shipNode = new Node2D { Name = "Ship" };
        AddChild(_shipNode);

        // Ship placeholder rectangle
        var shipRect = new ColorRect
        {
            Name = "ShipRect",
            Color = new Color(0.25f, 0.35f, 0.55f, 0.9f),
            Size = new Vector2(80, 50),
            Position = new Vector2(-40, -46),
        };
        _shipNode.AddChild(shipRect);

        // Damage label — above the ship, bobs with it
        _damageLabel = new Label
        {
            Name = "DamageLabel",
            Text = "0",
            HorizontalAlignment = HorizontalAlignment.Center,
            Size = new Vector2(80, 24),
            Position = new Vector2(-40, -76),
        };
        var damageBg = new StyleBoxFlat { BgColor = new Color(0.3f, 0.3f, 0.3f, 0.8f) };
        _damageLabel.AddThemeStyleboxOverride("normal", damageBg);
        _shipNode.AddChild(_damageLabel);

        // Parent hitbox — covers ammo square + fire button area, handles all input
        _hitbox = new Button
        {
            Name = "FireButton",
            Size = new Vector2(80,
                113), // 9 (ammo top) + 80 (ammo) + 24 (fire overlap into ammo = 77-9=68... just total: 77+36=113)
            Position = new Vector2(-40, 9),
        };
        _hitbox.AddThemeStyleboxOverride("normal", new StyleBoxEmpty());
        _hitbox.AddThemeStyleboxOverride("hover", new StyleBoxEmpty());
        _hitbox.AddThemeStyleboxOverride("pressed", new StyleBoxEmpty());
        _hitbox.AddThemeStyleboxOverride("disabled", new StyleBoxEmpty());
        _hitbox.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());
        _hitbox.Pressed += OnButtonPressed;
        _hitbox.MouseEntered += UpdateFireRectColor;
        _hitbox.MouseExited += UpdateFireRectColor;
        _hitbox.ButtonDown += () => _fireRect.Color = new Color(0.4f, 0.1f, 0.1f, 1f);
        _hitbox.ButtonUp += UpdateFireRectColor;
        AddChild(_hitbox);

        // Ammo count — inside hitbox, offset back to original root-relative position
        _ammoCountLabel = new Label
        {
            Name = "AmmoCount",
            Text = "0",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Size = new Vector2(80, 80),
            Position = new Vector2(0, 0),
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        var ammoBg = new StyleBoxFlat { BgColor = new Color(0.3f, 0.3f, 0.3f, 0.8f) };
        _ammoCountLabel.AddThemeStyleboxOverride("normal", ammoBg);
        _hitbox.AddChild(_ammoCountLabel);

        // Fire visual rect — higher z-index, fire label is its child
        _fireRect = new ColorRect
        {
            Name = "FireRect",
            Size = new Vector2(80, 36),
            Position = new Vector2(0, 68),
            ZIndex = 1,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };

        // Fire label — child of fire rect, position relative to rect
        _fireLabel = new Label
        {
            Name = "FireLabel",
            Text = "FIRE",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Size = new Vector2(80, 36),
            Position = new Vector2(0, 0),
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        _fireRect.AddChild(_fireLabel);
        _hitbox.AddChild(_fireRect);

        // Energy cost node — to the left of the fire rect, eventually becomes energy orb
        _energyCostLabel = new Label
        {
            Name = "EnergyCost",
            Text = "1",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Size = new Vector2(28, 28),
            Position = new Vector2(-18, 72),
            ZIndex = 2,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        var energyBg = new StyleBoxFlat { BgColor = new Color(0.15f, 0.15f, 0.5f, 0.9f), CornerRadiusTopLeft = 14, CornerRadiusTopRight = 14, CornerRadiusBottomLeft = 14, CornerRadiusBottomRight = 14 };
        _energyCostLabel.AddThemeStyleboxOverride("normal", energyBg);
        _hitbox.AddChild(_energyCostLabel);
    }

    public override void _EnterTree()
    {
        AmmoResource.AmmoChanged += OnAmmoChanged;
        CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
    }

    public override void _ExitTree()
    {
        AmmoResource.AmmoChanged -= OnAmmoChanged;
        CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
    }

    /// <summary>Called by NCreaturePatch after the node is added to the tree.</summary>
    public void Initialize(Player player)
    {
        _player = player;
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
        _shipNode.Position = new Vector2(0f, Mathf.Sin(_bobTime) * BobAmplitude);
    }

    // -------------------------------------------------------------------------
    // Event handlers
    // -------------------------------------------------------------------------

    private void OnAmmoChanged(PlayerCombatState state, int oldVal, int newVal)
    {
        if (!_initialized || state != _player.PlayerCombatState) return;

        if (!_hasEverHadAmmo && newVal > 0)
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

    private bool IsPlayerTurn =>
        NCombatRoom.Instance?.Ui.Hand.CurrentMode == NPlayerHand.Mode.Play;

    private bool CanFire =>
        _initialized
        && !_isFiring
        && _player.PlayerCombatState != null
        && AmmoResource.GetAmmo(_player) > 0
        && _player.PlayerCombatState.Energy >= AmmoResource.GetShotEnergyCost(_player)
        && IsPlayerTurn
        && !CombatManager.Instance.IsOverOrEnding;

    private void UpdateState()
    {
        if (!_initialized) return;
        if (_player.PlayerCombatState == null) return;

        var ammo = AmmoResource.GetAmmo(_player);
        _ammoCountLabel.Text = ammo.ToString();

        var damage = (int)AmmoResource.CalculateShotDamage(_player);
        _damageLabel.Text = $"dmg:{damage}";

        _energyCostLabel.Text = AmmoResource.GetShotEnergyCost(_player).ToString();

        var canFire = CanFire;
        _shipNode.Modulate = canFire ? Colors.White : new Color(0.5f, 0.5f, 0.5f, 1f);
        _hitbox.Disabled = !canFire;
        UpdateFireRectColor();
    }

    private void UpdateFireRectColor()
    {
        if (!_hitbox.Disabled && _hitbox.IsHovered())
            _fireRect.Color = new Color(0.6f, 0.2f, 0.2f, 0.9f); // hover
        else if (_hitbox.Disabled)
            _fireRect.Color = new Color(0.3f, 0.3f, 0.3f, 0.6f); // disabled
        else
            _fireRect.Color = new Color(0.8f, 0.3f, 0.2f, 1f); // normal
    }

    // -------------------------------------------------------------------------
    // Input
    // -------------------------------------------------------------------------

    private void OnButtonPressed()
    {
        if (!CanFire) return;

        _isFiring = true;
        UpdateState();

        var action = new FireAmmoAction(_player);
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(action);

        WaitForActionComplete(action);
    }

    private async void WaitForActionComplete(FireAmmoAction action)
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