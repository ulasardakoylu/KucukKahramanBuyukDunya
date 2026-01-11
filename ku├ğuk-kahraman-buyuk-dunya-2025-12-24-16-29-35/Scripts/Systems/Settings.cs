using Godot;

public partial class Settings : Control
{
    // Ekran
    private CheckBox fullscreenCheck;
    private OptionButton resolutionOption;
    private CheckBox vsyncCheck;

    // Ses
    private HSlider masterSlider;
    private HSlider musicSlider;
    private HSlider sfxSlider;

    // Oyun
    private OptionButton difficultyOption;
    private Button resetButton;


    // Geri
    private TextureButton backButton;

    public override void _Ready()
    {
        // ===== EKRAN =====
        fullscreenCheck = GetNode<CheckBox>("MarginContainer/VBoxContainer/TabContainer/Ekran/VBoxContainer/FullscreenCheck");
        resolutionOption = GetNode<OptionButton>("MarginContainer/VBoxContainer/TabContainer/Ekran/VBoxContainer/HBoxContainer/ResolutionOption");
        vsyncCheck = GetNode<CheckBox>("MarginContainer/VBoxContainer/TabContainer/Ekran/VBoxContainer/VSyncCheck");

        // ===== SES =====
        masterSlider = GetNode<HSlider>("MarginContainer/VBoxContainer/TabContainer/Ses/VBoxContainer/Master/MasterSlider");
        musicSlider = GetNode<HSlider>("MarginContainer/VBoxContainer/TabContainer/Ses/VBoxContainer/Music/MusicSlider");
        sfxSlider = GetNode<HSlider>("MarginContainer/VBoxContainer/TabContainer/Ses/VBoxContainer/SFX/SFXSlider");

        // ===== OYUN =====
        difficultyOption = GetNode<OptionButton>("MarginContainer/VBoxContainer/TabContainer/Oyun/VBoxContainer/HBoxContainer/DifficultyOption");
        resetButton = GetNode<Button>("MarginContainer/VBoxContainer/TabContainer/Oyun/VBoxContainer/HBoxContainer/ResetButton");

        

        // ===== GERİ =====
        backButton = GetNode<TextureButton>("BackButton");

        InitializeOptions();
        LoadCurrentSettings();
        ConnectSignals();
        ScaleUI();
        GD.Print("[SETTINGS] Ayarlar sayfası hazır!");
    }

    private void ScaleUI()
    {
        // BackButton'ı büyüt
        if (backButton != null)
        {
            backButton.CustomMinimumSize = new Vector2(300, 60);
            backButton.AddThemeFontSizeOverride("font_size", 22);
        }

        // Reset Button'ı büyüt
        if (resetButton != null)
        {
            resetButton.CustomMinimumSize = new Vector2(400, 60);
            resetButton.AddThemeFontSizeOverride("font_size", 20);
        }

        // Slider'ları büyüt
        if (masterSlider != null)
            masterSlider.CustomMinimumSize = new Vector2(450, 30);

        if (musicSlider != null)
            musicSlider.CustomMinimumSize = new Vector2(450, 30);

        if (sfxSlider != null)
            sfxSlider.CustomMinimumSize = new Vector2(450, 30);

        GD.Print("[SETTINGS] UI scaled!");
    }

    private void InitializeOptions()
    {
        // Çözünürlükler
        resolutionOption.AddItem("1280x720");
        resolutionOption.AddItem("1920x1080");
        resolutionOption.AddItem("2560x1440");

        // Zorluklar
        difficultyOption.AddItem("Kolay");
        difficultyOption.AddItem("Orta");
        difficultyOption.AddItem("Zor");
    }

    private void LoadCurrentSettings()
    {
        var profile = UserProfile.Instance;

        // Ekran
        fullscreenCheck.ButtonPressed = profile.IsFullscreen;
        vsyncCheck.ButtonPressed = profile.VSync;

        if (profile.Resolution.X == 1280)
            resolutionOption.Selected = 0;
        else if (profile.Resolution.X == 1920)
            resolutionOption.Selected = 1;
        else if (profile.Resolution.X == 2560)
            resolutionOption.Selected = 2;

        // Ses
        masterSlider.Value = profile.MasterVolume;
        musicSlider.Value = profile.MusicVolume;
        sfxSlider.Value = profile.SFXVolume;

        // Oyun
        difficultyOption.Selected = profile.Difficulty switch
        {
            "Kolay" => 0,
            "Orta" => 1,
            "Zor" => 2,
            _ => 1
        };

    }

    private void ConnectSignals()
    {
        fullscreenCheck.Toggled += OnFullscreenToggled;
        resolutionOption.ItemSelected += OnResolutionSelected;
        vsyncCheck.Toggled += OnVSyncToggled;

        masterSlider.ValueChanged += OnMasterVolumeChanged;
        musicSlider.ValueChanged += OnMusicVolumeChanged;
        sfxSlider.ValueChanged += OnSFXVolumeChanged;

        difficultyOption.ItemSelected += OnDifficultySelected;
        resetButton.Pressed += OnResetPressed;
        backButton.Pressed += OnBackPressed;
    }

    // ===== EKRAN AYARLARI =====
    private void OnFullscreenToggled(bool pressed)
    {
        UserProfile.Instance.IsFullscreen = pressed;
        UserProfile.Instance.ApplyDisplaySettings();
        UserProfile.Instance.SaveSettings();
    }

    private void OnResolutionSelected(long index)
    {
        Vector2I resolution = index switch
        {
            0 => new Vector2I(1280, 720),
            1 => new Vector2I(1920, 1080),
            2 => new Vector2I(2560, 1440),
            _ => new Vector2I(1280, 720)
        };

        UserProfile.Instance.Resolution = resolution;
        UserProfile.Instance.ApplyDisplaySettings();
        UserProfile.Instance.SaveSettings();
    }

    private void OnVSyncToggled(bool pressed)
    {
        UserProfile.Instance.VSync = pressed;
        UserProfile.Instance.ApplyDisplaySettings();
        UserProfile.Instance.SaveSettings();
    }

    // ===== SES AYARLARI =====
    private void OnMasterVolumeChanged(double value)
    {
        UserProfile.Instance.MasterVolume = (float)value;
        UserProfile.Instance.ApplyAudioSettings();
        UserProfile.Instance.SaveSettings();
    }

    private void OnMusicVolumeChanged(double value)
    {
        UserProfile.Instance.MusicVolume = (float)value;
        UserProfile.Instance.ApplyAudioSettings();
        UserProfile.Instance.SaveSettings();
    }

    private void OnSFXVolumeChanged(double value)
    {
        UserProfile.Instance.SFXVolume = (float)value;
        UserProfile.Instance.ApplyAudioSettings();
        UserProfile.Instance.SaveSettings();
    }

    // ===== OYUN AYARLARI =====
    private void OnDifficultySelected(long index)
    {
        string difficulty = index switch
        {
            0 => "Kolay",
            1 => "Orta",
            2 => "Zor",
            _ => "Orta"
        };

        UserProfile.Instance.Difficulty = difficulty;
        UserProfile.Instance.SaveSettings();
        GD.Print($"[SETTINGS] Zorluk: {difficulty}");
    }

    private void OnResetPressed()
    {
        var confirm = new ConfirmationDialog();
        confirm.DialogText = "Tüm ilerleme sıfırlanacak! Emin misiniz?";
        confirm.Title = "ONAY";
        confirm.OkButtonText = "EVET, SIFIRLA";
        confirm.CancelButtonText = "HAYIR";

        confirm.Confirmed += () =>
        {
            UserProfile.Instance.ResetAllProgress();
            GD.Print("[SETTINGS] İlerleme sıfırlandı!");
            confirm.QueueFree();
        };

        confirm.Canceled += () =>
        {
            GD.Print("[SETTINGS] Sıfırlama iptal edildi.");
            confirm.QueueFree();
        };

        AddChild(confirm);
        confirm.PopupCentered(new Vector2I(400, 150));
    }

    private void OnBackPressed()
    {
        if (GetTree().Root.HasMeta("ReturnToPause"))
        {
            string pausedLevel = (string)GetTree().Root.GetMeta("PausedLevel");

            GD.Print($"[SETTINGS] 🔙 Pause'a geri dönülüyor: {pausedLevel}");

            // Meta'ları temizle
            GetTree().Root.RemoveMeta("ReturnToPause");
            GetTree().Root.RemoveMeta("PausedLevel");

            // Level'e geri dön
            GetTree().ChangeSceneToFile(pausedLevel);
        }
        else
        {
            // Normal akış: Ana menüye git
            GetTree().ChangeSceneToFile("res://Resources/main_menu.tscn");
        }
    }
}