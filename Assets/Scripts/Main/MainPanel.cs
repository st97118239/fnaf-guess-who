using Mirror;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : NetworkBehaviour
{
    public Game game;
    public GameManager gameManager;
    public ListPanel listPanel;
    public PlayerPanel playerPanel;
    public PopupPaper popupPaper;
    public SettingsMenu settingsMenu;

    public PlayerPolaroid[] playerPolaroids;

    public Note versionNote;
    public Note readyNote;
    public Note connectionNote;
    public Note hostNote;
    public Note listCreatorNote;
    public Note settingsNote;
    public Note quitNote;

    public string username;
    public string avatar;
    public string[] selectedArray;
    public bool isHost;
    public bool isReady;
    public bool isInGame;

    [SerializeField] private List<Image> posters;
    [SerializeField] private List<Sprite> posterSprites;

    private readonly System.Random rnd = new();

    private void Awake()
    {
        versionNote.ChangeText("Version: " + gameManager.version);

        posterSprites = posterSprites.OrderBy(i => rnd.Next()).ToList();

        for (int i = 0; i < posters.Count; i++)
        {
            if (posterSprites.Count >= i + 1)
                posters[i].sprite = posterSprites[i];
            else
                posters[i].color = Color.clear;
        }
    }

    public void ReadyUp()
    {
        if (game.player.isLocalPlayer && !game.player.isReadyToPlay)
        {
            game.player.isReadyToPlay = true;
            isReady = true;
            Debug.Log("Player is ready.");
            readyNote.Disable();
            CreateArray();
        }
    }

    public void CreateArray()
    {
        List<string> charList = listPanel.selectedList.characters;
        selectedArray = charList.ToArray();

        gameManager.SetList(selectedArray);
    }

    public void SetPlayerPolaroid(bool fadeImage, bool fadeText)
    {
         playerPolaroids[0].Load(username, avatar, fadeImage, fadeText);
    }

    public void SetOpponentPolaroid()
    {
        if (gameManager?.opponent)
            playerPolaroids[1].Load(gameManager.opponent.username, gameManager.opponent.avatar, true, true);
        else
            playerPolaroids[1].Clear();
    }

    public void Host()
    {
        connectionNote.Disable();

        if (NetworkServer.active)
        {
            Debug.Log("Stopping host.");

            if (isReady)
                isReady = false;

            hostNote.Disable();
            game.gameManager.DisconnectAll();
        }
        else if (!NetworkServer.active)
        {
            connectionNote.Disable();

            Debug.Log("Starting host.");

            isHost = true;

            try
            {
                NetworkManager.singleton.StartHost();
                hostNote.ChangeText("Stop host");
            }
            catch (SocketException)
            {
                Debug.LogError("SocketException: Only one usage of each socket address (protocol/network address/port) is normally permitted.");
                popupPaper.Show(Error.MultipleHosts);
                Debug.Log("Stopping host.");

                connectionNote.ChangeText("Connect");

                readyNote.Disable();
                connectionNote.Enable();
                hostNote.Enable();
                quitNote.Enable();
                settingsMenu.isConnected = false;
            }
        }
    }

    public void Disconnected()
    {
        Debug.Log("Disconnected from server.");

        connectionNote.ChangeText("Connect");

        readyNote.Disable();
        connectionNote.Enable();
        hostNote.Enable();
        quitNote.Enable();
        settingsMenu.isConnected = false;
        listCreatorNote.Enable();

        if (isReady)
            isReady = false;
    }

    public void HostStop()
    {
        NetworkManager.singleton.StopServer();

        connectionNote.Disable();

        hostNote.ChangeText("Start host");
        hostNote.Enable();
        isHost = false;

        if (isReady)
            isReady = false;
    }

    public void Connect()
    {
        if (NetworkClient.isConnected)
        {
            Debug.Log("Disconnecting from server.");

            if (isReady)
                isReady = false;

            connectionNote.Disable();

            if (game.gameManager.player.playerIdx == 1)
                game.gameManager.player1.Disconnect();
            else if (game.gameManager.player.playerIdx == 2)
                game.gameManager.player2.Disconnect();
        }
        else if (!NetworkClient.isConnected)
        {
            connectionNote.Disable();
            hostNote.Disable();

            NetworkManager.singleton.StartClient();
            Debug.Log("Connecting to server.");

            Invoke(nameof(CheckConnection), 2);
        }
    }

    public void CheckConnection()
    {
        if (NetworkClient.isConnected)
            return;

        Debug.Log("Can't connect.");
        NetworkManager.singleton.StopClient();
        popupPaper.Show(Error.CantConnect);

        hostNote.Enable();
        connectionNote.Enable();
        connectionNote.ChangeText("Connect");

        if (isReady)
            isReady = false;
    }

    public void ServerFull()
    {
        if (NetworkClient.isConnected)
            return;

        CancelInvoke(nameof(CheckConnection));

        Debug.Log("Server is full.");
        NetworkManager.singleton.StopClient();
        popupPaper.Show(Error.ServerFull);

        hostNote.Enable();
        connectionNote.Enable();
        connectionNote.ChangeText("Connect");
        Invoke(nameof(CheckIfEnabled), 0.1f);
    }

    private void CheckIfEnabled()
    {
        if (hostNote.isCrossedOff)
            hostNote.Enable();
        if (connectionNote.isCrossedOff)
            connectionNote.Enable();
    }

    public void ClientConnected()
    {
        connectionNote.ChangeText("Disconnect");
        if (!isHost)
            connectionNote.Enable();

        quitNote.Disable();
        settingsMenu.isConnected = true;

        CancelInvoke(nameof(CheckConnection));
    }

    public void EnableReady()
    {
        readyNote.Enable();
    }

    public void DisabeReady()
    {
        readyNote.Disable();
    }

    public void Quit()
    {
        Debug.Log("Quitting game.");
        quitNote.Disable();

        listPanel.saveManager.Save();
        settingsMenu.Save();

        Invoke(nameof(ExitGame), 0.4f);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
