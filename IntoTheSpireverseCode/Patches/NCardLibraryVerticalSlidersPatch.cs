using HarmonyLib;
using Godot;
using System;
using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;

namespace IntoTheSpireverse.Patches;

[HarmonyPatch(typeof(NCardLibrary))]
public class NCardLibraryVerticalSlidersPatch
{
    private const string SliderScenePath = "res://scenes/screens/settings_slider.tscn";
    private static LineEdit? _defaultOutputFolderField;

    // ── UI refs ───────────────────────────────────────────────────────────
    private static PanelContainer? _sliderContainer;
    private static (Control Slider, Label ValueLabel) _row1;
    private static (Control Slider, Label ValueLabel) _row2;
    private static (Control Slider, Label ValueLabel) _row3;
    private static (Control Slider, Label ValueLabel) _rowR;
    private static (Control Slider, Label ValueLabel) _rowG;
    private static (Control Slider, Label ValueLabel) _rowB;
    private static (Control Slider, Label ValueLabel) _rowContrast;
    private static Button? _flipButton;
    private static PortraitSearchBox? _portraitSearch;

    // ── State ─────────────────────────────────────────────────────────────
    private static NCardHolder? _currentHolder;
    private static float _currentH        = 100f;
    private static float _currentS        = 100f;
    private static float _currentV        = 100f;
    private static float _currentR        = 100f;
    private static float _currentG        = 100f;
    private static float _currentB        = 100f;
    private static float _currentContrast = 100f;
    private static bool  _currentFlipH    = false;
    private static string _currentPortraitPath = "";

    private static FileDialog? _folderDialog;
    private static bool _loaded;
    
    // ── Setup ─────────────────────────────────────────────────────────────

    [HarmonyPatch("_Ready")]
    [HarmonyPostfix]
    // ReSharper disable once UnusedParameter.Local
    static void SetupSlidersOnInspector(NCardLibrary __instance)
    {
       // if (!IntoTheSpireverseConfig.ShowWipContent) return;
        if (_loaded) return;
        var inspectScreen = NGame.Instance!.GetInspectCardScreen();

        _sliderContainer = new PanelContainer();
        _sliderContainer.CustomMinimumSize = new Vector2(400, 400);
        _sliderContainer.Visible = false;
        _sliderContainer.SetAnchorsPreset(Control.LayoutPreset.CenterLeft);
        _sliderContainer.AddThemeStyleboxOverride("panel", new StyleBoxEmpty());

        var pos = _sliderContainer.Position;
        _sliderContainer.Position = new Vector2(pos.X + 80, pos.Y - 280);

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 10);
        vbox.Alignment = BoxContainer.AlignmentMode.Center;
        vbox.Position = new Vector2(vbox.Position.X + 40, vbox.Position.Y);
        _sliderContainer.AddChild(vbox);

        PackedScene sliderScene = GD.Load<PackedScene>(SliderScenePath);

        // HSV sliders
        _row1 = CreateSliderRow(vbox, sliderScene, "Hue",      val => { _currentH        = (float)val; UpdateCardShader(); });
        _row2 = CreateSliderRow(vbox, sliderScene, "Sat",      val => { _currentS        = (float)val; UpdateCardShader(); });
        if (_row2.Slider.GetNodeOrNull<Godot.Range>("Slider") is { } satSlider)
            satSlider.MaxValue = 200;
        _row3 = CreateSliderRow(vbox, sliderScene, "Lum",      val => { _currentV        = (float)val; UpdateCardShader(); });
        if (_row3.Slider.GetNodeOrNull<Godot.Range>("Slider") is { } lumSlider)
            lumSlider.MaxValue = 200;

        // RGB sliders
        _rowR = CreateSliderRow(vbox, sliderScene, "Red",      val => { _currentR        = (float)val; UpdateCardShader(); });
        if (_rowR.Slider.GetNodeOrNull<Godot.Range>("Slider") is { } redSlider)
            redSlider.MaxValue = 200;
        _rowG = CreateSliderRow(vbox, sliderScene, "Green",    val => { _currentG        = (float)val; UpdateCardShader(); });
        if (_rowG.Slider.GetNodeOrNull<Godot.Range>("Slider") is { } greenSlider)
            greenSlider.MaxValue = 200;
        _rowB = CreateSliderRow(vbox, sliderScene, "Blue",     val => { _currentB        = (float)val; UpdateCardShader(); });
        if (_rowB.Slider.GetNodeOrNull<Godot.Range>("Slider") is { } blueSlider)
            blueSlider.MaxValue = 200;

        // Contrast slider
        _rowContrast = CreateSliderRow(vbox, sliderScene, "Contrast", val => { _currentContrast = (float)val; UpdateCardShader(); });
        if (_rowContrast.Slider.GetNodeOrNull<Godot.Range>("Slider") is { } contrastSlider)
            contrastSlider.MaxValue = 200;

        // Flip X toggle
        _flipButton = new Button();
        _flipButton.Text = "Flip X: OFF";
        _flipButton.CustomMinimumSize = new Vector2(120, 40);
        _flipButton.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        _flipButton.Pressed += OnFlipButtonPressed;
        vbox.AddChild(_flipButton);

        _portraitSearch = new PortraitSearchBox(vbox);
        _portraitSearch.PortraitSelected += OnPortraitSelected;

        BuildButtonRow(vbox);

        inspectScreen.AddChild(_sliderContainer);
        inspectScreen.MoveChild(_sliderContainer, inspectScreen.GetChildCount() - 1);

        _defaultOutputFolderField.PlaceholderText = CardArtRoller.DefaultsOutputDirectory;
        
        _loaded = true;
    }

    private static void BuildButtonRow(VBoxContainer vbox)
    {
        var buttonRow = new HBoxContainer();
        buttonRow.AddThemeConstantOverride("separation", 10);
        buttonRow.Alignment = BoxContainer.AlignmentMode.Center;
        vbox.AddChild(buttonRow);

        AddButton(buttonRow, "Save",  80, OnSaveButtonPressed);
        AddButton(buttonRow, "Clear", 80, OnClearButtonPressed);

        bool isDevMode = OS.GetCmdlineArgs().Contains("--modder");

        var saveDefaultBtn = AddButton(vbox, "Save Default", 170, OnSaveDefaultButtonPressed);
        saveDefaultBtn.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        saveDefaultBtn.Visible = isDevMode;

        // Folder field (only visible in devMode)
        var folderRow = new HBoxContainer();
        folderRow.AddThemeConstantOverride("separation", 8);
        folderRow.Alignment = BoxContainer.AlignmentMode.Center;
        folderRow.Visible = isDevMode;
        vbox.AddChild(folderRow);

        _defaultOutputFolderField = new LineEdit();
        _defaultOutputFolderField.CustomMinimumSize = new Vector2(180, 0);
        _defaultOutputFolderField.PlaceholderText = "SaveDir/defaults";
        _defaultOutputFolderField.Editable = false;
        folderRow.AddChild(_defaultOutputFolderField);

        var browseBtn = new Button();
        browseBtn.Text = "...";
        browseBtn.CustomMinimumSize = new Vector2(40, 0);
        browseBtn.Pressed += OnBrowseFolderPressed;
        folderRow.AddChild(browseBtn);
    }
    
    private static void OnBrowseFolderPressed()
    {
        if (_folderDialog == null)
        {
            _folderDialog = new FileDialog();
            _folderDialog.FileMode = FileDialog.FileModeEnum.OpenDir;
            _folderDialog.Access = FileDialog.AccessEnum.Filesystem;
            _folderDialog.Title = "Select Default Output Folder";
            _folderDialog.DirSelected += OnFolderSelected;
            _sliderContainer!.AddChild(_folderDialog);
        }

        _folderDialog.PopupCentered(new Vector2I(600, 400));
    }

    private static void OnFolderSelected(string path)
    {

        if (_defaultOutputFolderField != null)
        { 
            CardArtRoller.DefaultsOutputDirectory = path;
            _defaultOutputFolderField.Text = path; 
            CardArtRoller.SaveConfig();
        }
    }

    private static Button AddButton(Node parent, string text, int width, Action onPressed)
    {
        var btn = new Button();
        btn.Text = text;
        btn.CustomMinimumSize = new Vector2(width, 40);
        btn.Pressed += onPressed;
        parent.AddChild(btn);
        return btn;
    }

    private static (Control Slider, Label ValueLabel) CreateSliderRow(
        VBoxContainer parent, PackedScene scene, string labelText, Action<double> onValueChanged)
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 15);
        row.Alignment = BoxContainer.AlignmentMode.End;
        parent.AddChild(row);

        var sliderInstance = (Control)scene.Instantiate();
        row.AddChild(sliderInstance);

        if (sliderInstance.GetNodeOrNull<Godot.Range>("Slider") is { } internalSlider)
        {
            if (sliderInstance.GetNodeOrNull<MegaLabel>("SliderValue") is { } sliderLabel)
            {
                sliderLabel.SetTextAutoSize(internalSlider.Value.ToString("0.0"));
                internalSlider.ValueChanged += val =>
                {
                    sliderLabel.SetTextAutoSize(val.ToString("0.0"));
                    onValueChanged.Invoke(val);
                };
            }
            
            internalSlider.CustomMinimumSize = new Vector2(10, internalSlider.CustomMinimumSize.Y);
        }
        sliderInstance.CustomMinimumSize = new Vector2(200, sliderInstance.CustomMinimumSize.Y);

        row.AddChild(new Control { CustomMinimumSize = new Vector2(15, 0) });

        var valLabel = new Label
        {
            Text = labelText,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            CustomMinimumSize = new Vector2(70, 0),
        };
        row.AddChild(valLabel);

        return (sliderInstance, valLabel);
    }

    // ── Show card ─────────────────────────────────────────────────────────

    [HarmonyPatch("ShowCardDetail")]
    [HarmonyPostfix]
    // ReSharper disable once UnusedMember.Local
    static void ShowSliders(NCardHolder holder)
    {
        _currentHolder = holder;

        if (_sliderContainer == null) return;
        if (!SaveManager.Instance.Progress.DiscoveredCards.Contains(holder.CardModel!.Id)) return;

        _sliderContainer.Visible = true;
        var parent = _sliderContainer.GetParent();
        parent.MoveChild(_sliderContainer, parent.GetChildCount() - 1);

        LoadHsvState(holder);
        SyncSlidersToState();
        RefreshPortraitTexture(holder);
    }

    private static void LoadHsvState(NCardHolder holder)
    {
        string cardId = holder.CardModel!.Id.ToString();

        var userHsv = CardArtRoller.GetCardData(cardId);
        if (userHsv != null)
        {
            SetState(
                userHsv.Hue * 100f, userHsv.Saturation * 100f, userHsv.Value * 100f,
                userHsv.Red * 100f, userHsv.Green * 100f, userHsv.Blue * 100f,
                userHsv.Contrast * 100f,
                userHsv.FlipH,
                userHsv.PortraitPath ?? "");
            return;
        }

        var defaultHsv = CardArtRoller.GetDefaultHsvForCard(cardId);
        if (defaultHsv != null)
        {
            SetState(
                defaultHsv.Hue * 100f, defaultHsv.Saturation * 100f, defaultHsv.Value * 100f,
                defaultHsv.Red * 100f, defaultHsv.Green * 100f, defaultHsv.Blue * 100f,
                defaultHsv.Contrast * 100f,
                defaultHsv.FlipH,
                defaultHsv.PortraitPath ?? "");
            return;
        }

        SetState(100f, 100f, 100f, 100f, 100f, 100f, 100f, false, "");
    }

    private static void SetState(float h, float s, float v, float r, float g, float b, float contrast, bool flipH, string portraitPath)
    {
        _currentH        = h;
        _currentS        = s;
        _currentV        = v;
        _currentR        = r;
        _currentG        = g;
        _currentB        = b;
        _currentContrast = contrast;
        _currentFlipH    = flipH;
        _currentPortraitPath = portraitPath;
    }

    private static void SyncSlidersToState()
    {
        if (_row1.Slider.GetNodeOrNull<Godot.Range>("Slider")        is { } s1)   s1.Value   = _currentH;
        if (_row2.Slider.GetNodeOrNull<Godot.Range>("Slider")        is { } s2)   s2.Value   = _currentS;
        if (_row3.Slider.GetNodeOrNull<Godot.Range>("Slider")        is { } s3)   s3.Value   = _currentV;
        if (_rowR.Slider.GetNodeOrNull<Godot.Range>("Slider")        is { } sR)   sR.Value   = _currentR;
        if (_rowG.Slider.GetNodeOrNull<Godot.Range>("Slider")        is { } sG)   sG.Value   = _currentG;
        if (_rowB.Slider.GetNodeOrNull<Godot.Range>("Slider")        is { } sB)   sB.Value   = _currentB;
        if (_rowContrast.Slider.GetNodeOrNull<Godot.Range>("Slider") is { } sCon) sCon.Value = _currentContrast;

        if (_flipButton != null)
            _flipButton.Text = _currentFlipH ? "Flip X: ON" : "Flip X: OFF";

        if (_portraitSearch != null)
            _portraitSearch.Text = !string.IsNullOrEmpty(_currentPortraitPath)
                ? Path.GetFileNameWithoutExtension(_currentPortraitPath)
                : "";
    }

    private static void RefreshPortraitTexture(NCardHolder? holder = null)
    {
        var portrait = GetInspectedPortrait();
        if (portrait == null) return;

        if (!string.IsNullOrEmpty(_currentPortraitPath) && ResourceLoader.Exists(_currentPortraitPath))
            portrait.Texture = ResourceLoader.Load<Texture2D>(_currentPortraitPath);
        else if (holder != null)
            portrait.Texture = ResourceLoader.Load<Texture2D>(holder.CardModel!.PortraitPath);
    }

    // ── Shader update ─────────────────────────────────────────────────────

    private static void UpdateCardShader()
    {
        if (_currentHolder == null) return;

        var portrait = GetInspectedPortrait();
        if (portrait == null) return;

        CardShaderHelper.ApplyToPortrait(
            portrait,
            _currentH / 100f, _currentS / 100f, _currentV / 100f,
            _currentR / 100f, _currentG / 100f, _currentB / 100f,
            _currentContrast / 100f);

        portrait.FlipH = _currentFlipH;
    }

    // ── Flip toggle ───────────────────────────────────────────────────────

    private static void OnFlipButtonPressed()
    {
        _currentFlipH = !_currentFlipH;
        if (_flipButton != null)
            _flipButton.Text = _currentFlipH ? "Flip X: ON" : "Flip X: OFF";
        UpdateCardShader();
    }

    // ── Portrait selection ────────────────────────────────────────────────

    private static void OnPortraitSelected(string path)
    {
        _currentPortraitPath = path;

        var portrait = GetInspectedPortrait();
        if (portrait != null && ResourceLoader.Exists(path))
            portrait.Texture = ResourceLoader.Load<Texture2D>(path);
    }

    // ── Buttons ───────────────────────────────────────────────────────────

    private static void OnSaveButtonPressed()
    {
        if (_currentHolder == null) return;

        string cardId = _currentHolder.CardModel!.Id.ToString();
        CardArtRoller.SaveHsvForCard(cardId, _currentH, _currentS, _currentV, _currentR, _currentG, _currentB, _currentContrast, _currentFlipH, _currentPortraitPath);
        ReloadCard();
        Log.Info($"[NCardLibraryVerticalSlidersPatch] Saved for card: {cardId}");
    }

    private static void OnClearButtonPressed()
    {
        if (_currentHolder == null) return;

        string cardId = _currentHolder.CardModel!.Id.ToString();
        CardArtRoller.DeleteHsvForCard(cardId);

        LoadHsvState(_currentHolder);
        SyncSlidersToState();
        RefreshPortraitTexture(_currentHolder);
        UpdateCardShader();
        ReloadCard();

        Log.Info($"[NCardLibraryVerticalSlidersPatch] Cleared for card: {cardId}");
    }

    private static void OnSaveDefaultButtonPressed()
    {
        if (_currentHolder == null) return;

        string cardId = _currentHolder.CardModel!.Id.ToString();
        CardArtRoller.SaveDefaultHsvForCard(cardId, _currentH, _currentS, _currentV, _currentR, _currentG, _currentB, _currentContrast, _currentFlipH, _currentPortraitPath);
        ReloadCard();
        Log.Info($"[NCardLibraryVerticalSlidersPatch] Saved default for card: {cardId}");
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private static TextureRect? GetInspectedPortrait()
    {
        var ncard = NGame.Instance!.GetInspectCardScreen().GetNodeOrNull<NCard>("Card");
        if (ncard.Model.Rarity == CardRarity.Ancient) return ncard.GetNodeOrNull<TextureRect>("%AncientPortrait");
        return ncard.GetNodeOrNull<TextureRect>("%Portrait");
    }

    private static void ReloadCard()
    {
        if (_currentHolder?.CardNode == null) return;
        AccessTools.Method(typeof(NCard), "Reload")?.Invoke(_currentHolder.CardNode, null);
    }
}