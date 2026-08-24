using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Actions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ui;

public partial class NAmmoButton : NShipDisplay
{
    private static readonly string _scenePath = IntoTheSpireverseResources.CaptainsShipScene;

    private readonly List<FireAmmoAction> _playQueue = [];

    private NAmmoCounter _ammoCounter = null!;
    private IntoTheSpireverseMegaLabel _fireLabel = null!;
    private IntoTheSpireverseMegaLabel _energyCostLabel = null!;
    private TextureRect _energyIcon = null!;
    private Control _fireButtonBackground = null!;
    private ComboControllerIcons _comboIcons = null!;

    private Tween? _bumpTween;

    protected override string? ClickedSfx => "event:/sfx/ui/clicks/ui_click";

    private int AvailableAmmoCount =>
        AmmoResource.GetAmmo(Player) - _playQueue.Count(a => a.State == GameActionState.WaitingForExecution);

    private bool CanFire
    {
        get
        {
            if (!_initialized || Player.PlayerCombatState == null ||
                Player.Creature.CombatState?.CurrentSide != CombatSide.Player)
                return false;
            if (AvailableAmmoCount <= 0) return false;
            if (AvailableEnergy < AmmoResource.GetShotEnergyCost(Player)) return false;

            var hasBigGuns = Player.Creature.HasPower<MassMunitionPower>();
            if (!hasBigGuns && !(Player.Creature.CombatState?.HittableEnemies.Any() ?? false))
                return false;
            return NCombatRoom.Instance?.Ui.Hand.CurrentMode == NPlayerHand.Mode.Play
                   && !CombatManager.Instance.IsOverOrEnding;
        }
    }

    private int AvailableEnergy
    {
        get
        {
            if (Player.PlayerCombatState == null) return 0;
            var pendingCost = _playQueue.Count(a => a.State == GameActionState.WaitingForExecution)
                              * AmmoResource.GetShotEnergyCost(Player);
            return Player.PlayerCombatState.Energy - pendingCost;
        }
    }

    #region Godot Lifecycle

    public override void _Ready()
    {
        base._Ready();

        _ammoCounter = GetNode<NAmmoCounter>("AmmoContainer/AmmoCounter");
        _fireLabel = GetNode<IntoTheSpireverseMegaLabel>("%FireButtonLabel");
        _energyCostLabel = GetNode<IntoTheSpireverseMegaLabel>("%EnergyLabel");
        _energyIcon = GetNode<TextureRect>("%EnergyIcon");
        _fireButtonBackground = GetNode<Control>("%FireButtonBackground");
        _comboIcons = new ComboControllerIcons(
            GetNode<TextureRect>("%ControllerIcon2"), // LT
            GetNode<TextureRect>("%ControllerIcon"), // A
            MegaInput.viewDrawPile,
            MegaInput.select,
            GetNode<IntoTheSpireverseMegaLabel>("%AddSymbol"));

        _comboIcons.Refresh();
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        RunManager.Instance.ActionQueueSet.ActionEnqueued += OnActionEnqueued;
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
        RunManager.Instance.ActionQueueSet.ActionEnqueued -= OnActionEnqueued;
        if (NControllerManager.Instance != null)
        {
            NControllerManager.Instance.ControllerDetected -= OnControllerChanged;
            NControllerManager.Instance.MouseDetected -= OnControllerChanged;
            NControllerManager.Instance.ControllerTypeChanged -= OnControllerChanged;
        }

        if (NInputManager.Instance != null)
            NInputManager.Instance.InputRebound -= OnControllerChanged;
        _playQueue.Clear();
    }

    private void OnControllerChanged() => _comboIcons?.Refresh(_isEnabled);

    #endregion

    #region Initialization

    public new static NAmmoButton Create()
    {
        var button = ResourceLoader.Load<PackedScene>(_scenePath).Instantiate<NAmmoButton>();
        ApplyDamageLabelFont(button);
        var font = PreloadManager.Cache.GetAsset<Font>(IntoTheSpireverseResources.MegaLabelFont);
        button.GetNode<NAmmoCounter>("AmmoContainer/AmmoCounter").ApplyFont(font, minSize: 32, maxSize: 32);
        ApplyFont(button.GetNode<IntoTheSpireverseMegaLabel>("%FireButtonLabel"),
            font, minSize: 20, maxSize: 20);
        ApplyFont(button.GetNode<IntoTheSpireverseMegaLabel>("%EnergyLabel"),
            font, minSize: 21, maxSize: 24);
        ApplyFont(button.GetNode<IntoTheSpireverseMegaLabel>("%AddSymbol"),
            font, minSize: 20, maxSize: 20);
        return button;
    }

    public override void Initialize(Player player)
    {
        Player = player;
        _energyIcon.Texture = PreloadManager.Cache.GetAsset<Texture2D>(
            EnergyIconHelper.GetPath(Player.Character.CardPool));
        _initialized = true;
        UpdateState();
    }

    private static void ApplyFont(MegaLabel label, Font font, int minSize, int maxSize)
    {
        label.AddThemeFontOverride(ThemeConstants.Label.Font, font);
        label.MinFontSize = minSize;
        label.MaxFontSize = maxSize;
    }

    #endregion

    #region Button Overrides

    protected override void OnFocus()
    {
        PlaySfx(HoveredSfx);
        ShowHoverTip();
        UpdateFireLabel();
        BumpFireButton(scale: new Vector2(1.25f, 1.25f));
    }

    protected override void OnUnfocus()
    {
        HideHoverTip();
        UpdateFireLabel();
        BumpFireButton(scale: Vector2.One, modulate: Colors.White);
    }

    protected override void OnPress()
    {
        PlaySfx(ClickedSfx);
        UpdateFireLabel();
        BumpFireButton(scale: new Vector2(0.9f, 0.9f), modulate: StsColors.red);
    }

    protected override void OnRelease()
    {
        if (!CanFire) return;

        BumpFireButton(scale: new Vector2(1.25f, 1.25f), modulate: Colors.White);

        var action = new FireAmmoAction(Player);
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(action);
        WaitForActionComplete(action);
    }
    
    private void BumpFireButton(Vector2 scale, Color? modulate = null)
    {
        _bumpTween?.Kill();
        _bumpTween = CreateTween();
        _bumpTween.SetParallel();
        _bumpTween.TweenProperty(_fireButtonBackground, "scale", scale, 0.05);
        if (modulate.HasValue)
        {
            _bumpTween.TweenProperty(_fireButtonBackground, "modulate", modulate.Value, 0.05);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _comboIcons?.Refresh();
        UpdateFireLabel();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _comboIcons?.Refresh(false);
        UpdateFireLabel();
    }

    private async void WaitForActionComplete(FireAmmoAction action)
    {
        while (action.State != GameActionState.Finished && action.State != GameActionState.Canceled)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        _playQueue.Remove(action);
        UpdateState();
    }

    #endregion

    #region Event Handlers

    private void OnActionEnqueued(GameAction action)
    {
        if (!_initialized) return;
        if (action is not FireAmmoAction ammoAction) return;
        if (ammoAction.OwnerId != Player.NetId) return;
        _playQueue.Add(ammoAction);
        UpdateState();
    }

    protected override void AnimIn()
    {
        Visible = true;
        DamageLabel.Visible = true;
        DamageIcon.Visible = true;
        FadeIn();
    }

    #endregion

    #region State Updates

    protected override void UpdateState()
    {
        base.UpdateState();
        if (!_initialized) return;
        if (Player.PlayerCombatState == null) return;

        _ammoCounter.SetCount(AvailableAmmoCount);
        _energyCostLabel.Text = AmmoResource.GetShotEnergyCost(Player).ToString();

        ShipContainer.Modulate = CanFire ? Colors.White : new Color(0.5f, 0.5f, 0.5f);
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

    #endregion
}