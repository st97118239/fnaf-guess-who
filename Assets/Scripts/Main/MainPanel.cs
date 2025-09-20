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
    public AudioManager audioManager;
    public ListPanel listPanel;
    public PlayerPanel playerPanel;
    public PopupPaper popupPaper;
    public SettingsMenu settingsMenu;
    public SaveManager saveManager;
    public DevManager devManager;
    public Animator animator;

    public Panels currentPanel;

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
    public bool hasLevelledUp;

    [SerializeField] private List<Image> posters;
    [SerializeField] private List<Sprite> posterSprites;

    private readonly System.Random rnd = new();

    private static readonly int GameClose = Animator.StringToHash("GameClose");

    private void Update()
    {
        switch (currentPanel)
        {
            case Panels.MainPanel when gameManager.opponent && gameManager.opponent.isReadyToPlay:
                playerPolaroids[1].Ready(true);
                break;
            case Panels.MainPanel when !gameManager.opponent && playerPolaroids[1].isReady:
                playerPolaroids[1].Ready(false);
                break;
        }
    }

    public void SpawnPosters()
    {
        posterSprites = posterSprites.OrderBy(i => rnd.Next()).ToList();

        for (int i = 0; i < posters.Count; i++)
        {
            if (posterSprites.Count >= i + 1)
                posters[i].sprite = posterSprites[i];
            else
                posters[i].color = Color.clear;
        }
    }

    public void Load()
    {
        SpawnPosters();

        currentPanel = Panels.MainPanel;
        animator.SetTrigger(GameClose);

        settingsMenu.isConnected = false;

        playerPolaroids[0].Ready(false);
        playerPolaroids[1].Ready(false);
        SetOpponentPolaroid();
        readyNote.Disable();
        connectionNote.Enable();
        hostNote.Enable();
        quitNote.Enable();
        connectionNote.ChangeText("Connect");

        isInGame = false;

        if (hasLevelledUp)
            Invoke(nameof(LevelledUp), 1.2f);
    }

    private void LevelledUp()
    {
        popupPaper.ShowPopup(PopupTextType.LevelUp);
        hasLevelledUp = false;
    }

    public void ReadyUp()
    {
        if (!game.player.isLocalPlayer || game.player.isReadyToPlay) 
            return;

        game.player.isReadyToPlay = true;
        isReady = true;
        Debug.Log("Player is ready.");
        readyNote.Checkmark();
        CreateArray();
        playerPolaroids[0].Ready(true);
    }

    public void CreateArray()
    {
        List<string> charList = listPanel.selectedList.characters;
        selectedArray = charList.ToArray();

        gameManager.SetList(selectedArray);
    }

    public void SetPlayerPolaroid(bool fadeImage, bool fadeName, bool fadeLevel)
    {
         playerPolaroids[0].Load(null, false, fadeImage, fadeName, fadeLevel);
    }

    public void SetOpponentPolaroid()
    {
        if (gameManager?.opponent)
            playerPolaroids[1].Load(gameManager.opponent, true, true, true, true);
        else
            playerPolaroids[1].Clear();
    }

    public void Host()
    {
        connectionNote.Disable();

        switch (NetworkServer.active)
        {
            case true:
            {
                Debug.Log("Stopping host.");

                if (isReady)
                    isReady = false;

                popupPaper.canShow = false;
                hostNote.Disable();
                game.gameManager.DisconnectAll();
                Invoke(nameof(TurnPopupPaperBackOn), 1f);
                break;
            }
            case false:
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
                    popupPaper.ShowError(Error.MultipleHosts);
                    Debug.Log("Stopping host.");

                    connectionNote.ChangeText("Connect");

                    playerPolaroids[0].Ready(false);
                    playerPolaroids[1].Ready(false);
                    readyNote.Disable();
                    connectionNote.Enable();
                    hostNote.Enable();
                    quitNote.Enable();
                    settingsMenu.isConnected = false;
                }

                break;
        }
    }
    
    private void TurnPopupPaperBackOn()
    {
        popupPaper.canShow = true;
    }

    public void Disconnected()
    {
        Debug.Log("Disconnected from server.");

        connectionNote.ChangeText("Connect");

        playerPolaroids[0].Ready(false);
        playerPolaroids[1].Ready(false);
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

        Debug.Log("Host stopped.");

        connectionNote.Enable();

        hostNote.ChangeText("Start host");
        hostNote.Enable();
        isHost = false;

        if (isReady)
            isReady = false;
    }

    public void Connect()
    {
        switch (NetworkClient.isConnected)
        {
            case true:
            {
                Debug.Log("Disconnecting from server.");

                if (isReady)
                    isReady = false;

                connectionNote.Disable();

                switch (game.gameManager.player.playerIdx)
                {
                    case 1:
                        game.gameManager.player1.Disconnect();
                        break;
                    case 2:
                        game.gameManager.player2.Disconnect();
                        break;
                }
                break;
            }
            case false:
                connectionNote.Disable();
                hostNote.Disable();

                NetworkManager.singleton.StartClient();
                Debug.Log("Connecting to server.");

                Invoke(nameof(CheckConnection), 2);
                break;
        }
    }

    public void CheckConnection()
    {
        if (NetworkClient.isConnected)
            return;

        Debug.Log("Can't connect.");
        NetworkManager.singleton.StopClient();
        popupPaper.ShowError(Error.CantConnect);

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
        popupPaper.ShowError(Error.ServerFull);

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

    public void DisableReady()
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
