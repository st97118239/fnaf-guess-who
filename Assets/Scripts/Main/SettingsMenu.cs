using kcp2k;
using Mirror;
using System.IO;
using TMPro;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public Settings settings;

    public bool isConnected;

    [SerializeField] private NetworkManagerScript networkManager;
    [SerializeField] private KcpTransport transport;

    [SerializeField] private GameObject settingsClipboard;
    [SerializeField] private Animator clipboardAnimator;
    [SerializeField] private GameObject backgroundBlocker;

    [SerializeField] private TMP_InputField serverAddressField;
    [SerializeField] private TMP_InputField serverPortField;

    [SerializeField] private string defaultServerAddress;
    [SerializeField] private string defaultServerPort;

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

        string save = JsonUtility.ToJson(settings);

        File.WriteAllText(settingsPath, save);

        Debug.Log("Saved settings.");
    }

    private void CreateNewFile()
    {
        settings = new();
        settings.serverAddress = defaultServerAddress;
        settings.serverPort = defaultServerPort;
    }

    private void LoadSettings()
    {
        settings.serverAddress ??= defaultServerAddress;
        networkManager.networkAddress = settings.serverAddress;
        serverAddressField.text = settings.serverAddress;

        settings.serverPort ??= defaultServerPort;
        transport.port = ushort.Parse(settings.serverPort);
        serverPortField.text = settings.serverPort;

        Debug.Log("Loaded settings.");
    }
}
