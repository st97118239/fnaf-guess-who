using kcp2k;
using Mirror;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Settings settings;

    public bool isConnected;

    [SerializeField] private NetworkManagerScript networkManager;
    [SerializeField] private KcpTransport transport;
    [SerializeField] private AudioManager audioManager;

    [SerializeField] private GameObject settingsClipboard;
    [SerializeField] private Animator clipboardAnimator;
    [SerializeField] private GameObject backgroundBlocker;

    [SerializeField] private TMP_InputField serverAddressField;
    [SerializeField] private TMP_InputField serverPortField;
    [SerializeField] private Slider soundEffectsSlider;
    [SerializeField] private Slider voicelinesSlider;
    [SerializeField] private Slider musicSlider;

    private string settingsPath;

    private void Start()
    {
        settingsPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Settings.json";

        if (!File.Exists(settingsPath))
        {
            CreateNewFile();
            return;
        }

        string json = File.ReadAllText(settingsPath);

        Settings save = JsonUtility.FromJson<Settings>(json);
        settings = save;

        LoadSettings();
    }

    public void OpenSettings()
    {
        LoadSettings();
        clipboardAnimator.SetTrigger("PaperOpen");
        backgroundBlocker.SetActive(true);

        if (isConnected)
        {
            serverAddressField.interactable = false;
            serverPortField.interactable = false;
        }
        else
        {
            serverAddressField.interactable = true;
            serverPortField.interactable = true;
        }
    }

    public void CloseSettings()
    {
        clipboardAnimator.SetTrigger("PaperClose");
        backgroundBlocker.SetActive(false);
        Save();
        LoadSettings();
    }

    public void Save()
    {
        settings.serverAddress = serverAddressField.text;
        settings.serverPort = serverPortField.text;
        settings.soundEffects = soundEffectsSlider.value;
        settings.voicelines = voicelinesSlider.value;
        settings.music = musicSlider.value;

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

        Debug.Log("Loaded settings.");
    }
}
