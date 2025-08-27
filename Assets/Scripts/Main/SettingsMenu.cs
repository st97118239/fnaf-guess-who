using kcp2k;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Settings settings;

    public bool isConnected;

    [SerializeField] private MainPanel mainPanel;
    [SerializeField] private NetworkManagerScript networkManager;
    [SerializeField] private KcpTransport transport;
    [SerializeField] private AudioManager audioManager;

    [SerializeField] private GameObject settingsClipboard;
    [SerializeField] private Animator clipboardAnimator;
    [SerializeField] private GameObject backgroundBlocker;

    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField serverAddressField;
    [SerializeField] private TMP_InputField serverPortField;
    [SerializeField] private Slider soundEffectsSlider;
    [SerializeField] private Slider voicelinesSlider;
    [SerializeField] private Slider musicSlider;

    private string settingsPath;
    private bool isShown;

    private void Start()
    {
        settingsPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Settings.json";

        if (!File.Exists(settingsPath))
            CreateNewFile();

        string json = File.ReadAllText(settingsPath);

        Settings save = JsonUtility.FromJson<Settings>(json);
        settings = save;

        LoadSettings();

        mainPanel.SetPlayerPolaroid(true, true);
    }

    private void Update()
    {
        if (isShown && Input.GetKeyDown(KeyCode.Escape))
            CloseSettings();
    }

    public void OpenSettings()
    {
        LoadSettings();

        if (isConnected)
        {
            usernameField.interactable = false;
            serverAddressField.interactable = false;
            serverPortField.interactable = false;
        }
        else
        {
            usernameField.interactable = true;
            serverAddressField.interactable = true;
            serverPortField.interactable = true;
        }

        clipboardAnimator.SetTrigger("PaperOpen");
        backgroundBlocker.SetActive(true);
        audioManager.soundEffects.PlayOneShot(audioManager.clipboardSFX);
        isShown = true;
    }

    public void CloseSettings()
    {
        clipboardAnimator.SetTrigger("PaperClose");
        audioManager.soundEffects.PlayOneShot(audioManager.clipboardSFX);
        Invoke(nameof(DisableBackground), 0.6f);
        Save();
        LoadSettings();

        isShown = false;
    }

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }

    public void Save()
    {
        settings.username = usernameField.text;
        settings.avatar = mainPanel.avatar;
        settings.serverAddress = serverAddressField.text;
        settings.serverPort = serverPortField.text;
        settings.soundEffects = soundEffectsSlider.value;
        settings.voicelines = voicelinesSlider.value;
        settings.music = musicSlider.value;
        settings.categoryIdx = mainPanel.listPanel.charactersPanel.categoryIdx;

        string save = JsonUtility.ToJson(settings);

        File.WriteAllText(settingsPath, save);

        Debug.Log("Saved settings.");
    }

    private void CreateNewFile()
    {
        settings = new();

        string save = JsonUtility.ToJson(settings);

        File.WriteAllText(settingsPath, save);

        Debug.Log("No settings file found. New settings file created.");
    }

    private void LoadSettings()
    {
        mainPanel.username = settings.username;
        usernameField.text = settings.username;

        mainPanel.avatar = settings.avatar;

        networkManager.networkAddress = settings.serverAddress;
        serverAddressField.text = settings.serverAddress;

        transport.port = ushort.Parse(settings.serverPort);
        serverPortField.text = settings.serverPort;

        audioManager.soundEffects.volume = settings.soundEffects;
        soundEffectsSlider.value = settings.soundEffects;

        audioManager.voicelines.volume = settings.voicelines;
        voicelinesSlider.value = settings.voicelines;

        audioManager.music.volume = settings.music;
        musicSlider.value = settings.music;

        mainPanel.listPanel.charactersPanel.categoryIdx = settings.categoryIdx;

        if (isShown)
            mainPanel.SetPlayerPolaroid(false, true);

        Debug.Log("Loaded settings.");
    }

    public void UpdateVolume()
    {
        audioManager.soundEffects.volume = soundEffectsSlider.value;

        audioManager.voicelines.volume = voicelinesSlider.value;

        audioManager.music.volume = musicSlider.value;
    }
}
