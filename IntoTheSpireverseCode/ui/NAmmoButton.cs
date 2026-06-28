using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Actions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ui;

public partial class NAmmoButton : NButton
{
    private static readonly string _scenePath = "res://IntoTheSpireverse/scenes/CaptainsShip.tscn";

    private static readonly AccessTools.FieldRef<CardModel, CardUpgradePreviewType> _upgradePreviewTypeRef =
        AccessTools.FieldRefAccess<CardModel, CardUpgradePreviewType>("_upgradePreviewType");

    private static readonly string _megaLabelFont = "res://themes/kreon_bold_glyph_space_one.tres";

    private Player _player = null!;
    private bool _initialized;
    private bool _hasEverHadAmmo;
    private readonly List<FireAmmoAction> _playQueue = [];

    private Control _shipContainer = null!;
    private ShaderMaterial? _hologramMaterial;
    private IntoTheSpireverseMegaRichTextLabel _damageLabel = null!;
    private IntoTheSpireverseMegaLabel _ammoCountLabel = null!;
    private IntoTheSpireverseMegaLabel _fireLabel = null!;
    private IntoTheSpireverseMegaLabel _energyCostLabel = null!;
    private TextureRect _energyIcon = null!;
    private TextureRect _damageIcon = null!;
    private Control _fireButtonBackground = null!;
    private ComboControllerIcons _comboIcons = null!;

    private Tween? _fadeTween;
    private Tween? _bumpTween;

    private float _bobTime;
    private const float BobAmplitude = 5f;
    private const float BobFrequency = 2f;

    protected override string? ClickedSfx => "event:/sfx/ui/clicks/ui_click";
    protected override string? HoveredSfx => "event:/sfx/ui/clicks/ui_hover";

    private CardModel? PhantomCard => AmmoResource.GetOrCreateState(_player)?.PhantomCard;

    private int AvailableAmmoCount =>
        AmmoResource.GetAmmo(_player) - _playQueue.Count(a => a.State == GameActionState.WaitingForExecution);

    private bool CanFire
    {
        get
        {
            if (!_initialized || _player.PlayerCombatState == null ||
                _player.Creature.CombatState?.CurrentSide != CombatSide.Player)
                return false;
            if (PhantomCard == null) return false;
            if (AvailableAmmoCount <= 0) return false;
            var hasBigGuns = _player.Creature.HasPower<BigGunsPower>();
            if (!hasBigGuns && !(_player.Creature.CombatState?.HittableEnemies.Any() ?? false))
                return false;
            return AvailableEnergy >= AmmoResource.GetShotEnergyCost(_player)
                   && NCombatRoom.Instance?.Ui.Hand.CurrentMode == NPlayerHand.Mode.Play
                   && !CombatManager.Instance.IsOverOrEnding;
        }
    }

    private int AvailableEnergy
    {
        get
        {
            if (_player.PlayerCombatState == null) return 0;
            var pendingCost = _playQueue.Count(a => a.State == GameActionState.WaitingForExecution)
                              * AmmoResource.GetShotEnergyCost(_player);
            return _player.PlayerCombatState.Energy - pendingCost;
        }
    }

    #region Godot Lifecycle

    public override void _Ready()
    {
        _shipContainer = GetNode<Control>("ShipContainer");
        _hologramMaterial = GetNode<TextureRect>("ShipContainer/ShipIcon").Material as ShaderMaterial;
        _damageLabel = GetNode<IntoTheSpireverseMegaRichTextLabel>("%DamageLabel");
        _ammoCountLabel = GetNode<IntoTheSpireverseMegaLabel>("%Count");
        _fireLabel = GetNode<IntoTheSpireverseMegaLabel>("%FireButtonLabel");
        _energyCostLabel = GetNode<IntoTheSpireverseMegaLabel>("%EnergyLabel");
        _energyIcon = GetNode<TextureRect>("%EnergyIcon");
        _damageIcon = GetNode<TextureRect>("%DamageIcon");
        _fireButtonBackground = GetNode<Control>("%FireButtonBackground");
        _comboIcons = new ComboControllerIcons(
            GetNode<TextureRect>("%ControllerIcon2"), // LT
            GetNode<TextureRect>("%ControllerIcon"), // A
            MegaInput.viewDrawPile,
            MegaInput.select,
            GetNode<IntoTheSpireverseMegaLabel>("%AddSymbol"));

        ConnectSignals();
        _comboIcons.Refresh();

        Modulate = new Color(1, 1, 1, 0);
        Visible = false;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
        RunManager.Instance.ActionQueueSet.ActionEnqueued += OnActionEnqueued;
        AmmoResource.AmmoChanged += OnAmmoChanged;
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
        CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
        RunManager.Instance.ActionQueueSet.ActionEnqueued -= OnActionEnqueued;
        AmmoResource.AmmoChanged -= OnAmmoChanged;
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

    public override void _Process(double delta)
    {
        if (!_initialized) return;
        _bobTime += (float)delta * BobFrequency;
        var bobY = Mathf.Sin(_bobTime) * BobAmplitude;
        _shipContainer.Position = new Vector2(
            _shipContainer.Position.X,
            bobY);
        var containerHeight = _shipContainer.Size.Y;
        if (containerHeight > 0f)
            _hologramMaterial?.SetShaderParameter("uvOffsetY", bobY / containerHeight);
    }

    #endregion

    #region Initialization

    public static NAmmoButton Create()
    {
        var button = ResourceLoader.Load<PackedScene>(_scenePath).Instantiate<NAmmoButton>();
        var font = PreloadManager.Cache.GetAsset<Font>(_megaLabelFont);
        ApplyFont(button.GetNode<IntoTheSpireverseMegaRichTextLabel>("%DamageLabel"), font,
            minSize: 22,
            maxSize: 28);
        ApplyFont(button.GetNode<IntoTheSpireverseMegaLabel>("%Count"), font, minSize: 32,
            maxSize: 32);
        ApplyFont(button.GetNode<IntoTheSpireverseMegaLabel>("%FireButtonLabel"),
            font, minSize: 20, maxSize: 20);
        ApplyFont(button.GetNode<IntoTheSpireverseMegaLabel>("%EnergyLabel"),
            font, minSize: 21, maxSize: 24);
        ApplyFont(button.GetNode<IntoTheSpireverseMegaLabel>("%AddSymbol"),
            font, minSize: 20, maxSize: 20);
        return button;
    }

    public void Initialize(Player player)
    {
        _player = player;
        _energyIcon.Texture = PreloadManager.Cache.GetAsset<Texture2D>(
            EnergyIconHelper.GetPath(_player.Character.CardPool));
        _initialized = true;
        UpdateState();
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

    #endregion

    #region Button Overrides

    protected override void OnFocus()
    {
        base.OnFocus(); // plays HoveredSfx
        UpdateFireLabel();
        _bumpTween?.Kill();
        _bumpTween = CreateTween();
        _bumpTween.TweenProperty(_fireButtonBackground, "scale", new Vector2(1.25f, 1.25f), 0.05);
        if (PhantomCard == null) return;

        //I've opted to go with the fieldref pattern here as I don't think it wise to publicize this generally.
        //This is an issue with how UpdateDynamicVarPreview only updates dynamicvars if the card is in hand or in play (CardUpgradePreviewType.Combat).
        //The best solution would be to transpile the CardModel.UpdateDynamicVarPreview method, but this workaround is a lot less work and is only slightly more disgusting. 
        _upgradePreviewTypeRef(PhantomCard) = CardUpgradePreviewType.Combat;
        NHoverTipSet.CreateAndShow(this,
                [HoverTipFactory.FromCard(PhantomCard), .. PhantomCard.HoverTips])
            ?.SetAlignment(this, HoverTipAlignment.Left);
        _upgradePreviewTypeRef(PhantomCard) = CardUpgradePreviewType.None;
    }

    protected override void OnUnfocus()
    {
        UpdateFireLabel();
        NHoverTipSet.Remove(this);
        _bumpTween?.Kill();
        _bumpTween = CreateTween();
        _bumpTween.SetParallel();
        _bumpTween.TweenProperty(_fireButtonBackground, "scale", Vector2.One, 0.05);
        _bumpTween.TweenProperty(_fireButtonBackground, "modulate", Colors.White, 0.05);
    }

    protected override void OnPress()
    {
        base.OnPress(); // plays ClickedSfx
        UpdateFireLabel();
        _bumpTween?.Kill();
        _bumpTween = CreateTween();
        _bumpTween.TweenProperty(_fireButtonBackground, "scale", new Vector2(0.9f, 0.9f), 0.05);
        _bumpTween.TweenProperty(_fireButtonBackground, "modulate", StsColors.red, 0.05);
    }

    protected override void OnRelease()
    {
        if (!CanFire) return;

        _bumpTween?.Kill();
        _bumpTween = CreateTween();
        _bumpTween.SetParallel();
        _bumpTween.TweenProperty(_fireButtonBackground, "scale", new Vector2(1.25f, 1.25f), 0.05);
        _bumpTween.TweenProperty(_fireButtonBackground, "modulate", Colors.White, 0.05);

        var action = new FireAmmoAction(_player);
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(action);
        WaitForActionComplete(action);
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

    private void OnAmmoChanged(PlayerCombatState pcs, int oldVal, int newVal)
    {
        if (!_initialized || pcs != _player.PlayerCombatState) return;
        if (!_hasEverHadAmmo && newVal > 0)
        {
            _hasEverHadAmmo = true;
            AnimIn();
        }
        UpdateState();
    }

    private void OnCombatStateChanged(CombatState state) => UpdateState();

    private void OnActionEnqueued(GameAction action)
    {
        if (!_initialized) return;
        if (action is not FireAmmoAction ammoAction) return;
        if (ammoAction.OwnerId != _player.NetId) return;
        _playQueue.Add(ammoAction);
        UpdateState();
    }

    private void AnimIn()
    {
        Visible = true;
        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "modulate:a", 1f, 0.3f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);
    }

    #endregion

    #region State Updates

    private void UpdateState()
    {
        if (!_initialized) return;
        if (_player.PlayerCombatState == null) return;

        _ammoCountLabel.Text = AvailableAmmoCount.ToString();

        var card = PhantomCard ?? ModelDb.Card<AmmoVolley>();
        var preHookDamage = card.DynamicVars.Damage.BaseValue;
        var damage = (int)Hook.ModifyDamage(
            _player.RunState,
            _player.Creature.CombatState,
            null,
            _player.Creature,
            preHookDamage,
            ValueProp.Move,
            card,
            ModifyDamageHookType.All,
            CardPreviewMode.Normal,
            out _);
        _damageLabel.Text = $"{damage}";
        _damageIcon.Texture = GetAttackIntentTexture(damage);
        _energyCostLabel.Text = AmmoResource.GetShotEnergyCost(_player).ToString();

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
        return PreloadManager.Cache.GetAsset<Texture2D>(
            ImageHelper.GetImagePath($"packed/intents/attack/intent_attack_{tier}.png"));
    }

    #endregion
}