using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar] public string username;
    [SyncVar] public string avatar;
    [SyncVar] public int wins;
    [SyncVar] public int games;
    [SyncVar] public string chosenCharacter;
    [SyncVar] public int playerIdx;
    [SyncVar] public bool isDev;
    [SyncVar] public bool isHost;
    [SyncVar] public bool isReadyToPlay;
    [SyncVar] public string accusedCharacter;

    public Game game;
    public MainPanel mainPanel;

    public bool forcedLeave;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gameManager.NewPlayer(this);

        if (isLocalPlayer)
        {
            game = FindFirstObjectByType<Game>();
            mainPanel = game.mainPanel;
            username = mainPanel.username;
            avatar = mainPanel.avatar;
            wins = PlayerPrefs.GetInt("Wins");
            games = PlayerPrefs.GetInt("Games");
            isDev = mainPanel.devManager.isUnlocked;
            if (isServer)
                isHost = mainPanel.isHost;
            game.player = this;
            gameManager.player = this;

            Invoke(nameof(SetGameManagerVariables), 0.3f);

            Debug.Log("Player object connected.");
        }
        else if (!isServer)
        {
            if (gameManager.player1 == gameManager.player)
            {
                gameManager.opponent = gameManager.player2;
            }
            else if (gameManager.player2 == gameManager.player)
            {
                gameManager.opponent = gameManager.player1;
            }
        }
        else if (isServer && isClient)
        {
            if (gameManager.player1 == gameManager.player)
            {
                gameManager.opponent = gameManager.player2;
            }
        }
    }

    private void SetGameManagerVariables()
    {
        if (playerIdx == 2)
            gameManager.opponent = gameManager.player1;

        gameManager.CmdVersionCheck(playerIdx, gameManager.version);
        gameManager.CmdSetPolaroidVariables();
    }

    public void Disconnect()
    {
        Invoke(nameof(RemoveConnection), 0.5f);
    }

    public void ForceDisconnect()
    {
        forcedLeave = true;

        Invoke(nameof(RemoveConnection), 0.5f);
    }

    [ClientRpc]
    public void RpcVersionDisconnect()
    {
        if (playerIdx == 2 && isLocalPlayer)
            Invoke(nameof(VersionRemoveDisconnect), 0.2f);
    }

    [ClientRpc]
    public void RpcForceDisconnect()
    {
        if (!isLocalPlayer)
            return;

        forcedLeave = true;

        Invoke(nameof(ForceRemoveConnection), 0.2f);
    }

    public void CanReady()
    {
        Debug.Log("Enabling ready.");
        mainPanel.EnableReady();
    }

    public void CanNotReady()
    {
        Debug.Log("Disabling ready");
        mainPanel.DisabeReady();
    }
    
    public void OpponentLeft()
    {
        isReadyToPlay = false;
        mainPanel.popupPaper.Show(Error.OpponentLeft);
    }

    public void RemoveConnection()
    {
        gameManager.player = null;
        gameManager.opponent = null;

        NetworkManager.singleton.StopClient();
        Debug.Log("Disconnected from server.");

        if (!mainPanel)
            return;

        if (forcedLeave)
            mainPanel.popupPaper.Show(Error.ServerFull);

        mainPanel.connectionNote.ChangeText("Connect");

        mainPanel.playerPolaroids[0].Ready(false);
        mainPanel.playerPolaroids[1].Ready(false);
        mainPanel.readyNote.Disable();
        mainPanel.connectionNote.Enable();
        mainPanel.hostNote.Enable();
        mainPanel.quitNote.Enable();
        mainPanel.settingsMenu.isConnected = false;
        mainPanel.SetOpponentPolaroid();

        gameManager.ResetGame();
    }

    public void VersionRemoveDisconnect()
    {
        gameManager.player = null;
        gameManager.opponent = null;

        NetworkManager.singleton.StopClient();
        Debug.Log("Disconnected from server.");

        if (!mainPanel)
            return;

        mainPanel.popupPaper.Show(Error.WrongVersion);

        mainPanel.connectionNote.ChangeText("Connect");

        mainPanel.readyNote.Disable();
        mainPanel.connectionNote.Enable();
        mainPanel.hostNote.Enable();
        mainPanel.quitNote.Enable();
        mainPanel.settingsMenu.isConnected = false;
        mainPanel.SetOpponentPolaroid();

        gameManager.ResetGame();
    }

    public void ForceRemoveConnection()
    {
        game.gameManager.player = null;

        if (gameManager.hasStarted)
        {
            game.Leave();
        }

        gameManager.player = null;
        gameManager.opponent = null;

        if (gameManager.hasStarted)
            mainPanel.popupPaper.Show(Error.OpponentLeft);
        else if (!isHost)
            mainPanel.popupPaper.Show(Error.HostLeft);

        NetworkManager.singleton.StopClient();
        Debug.Log("Disconnected from server.");

        if (!mainPanel)
            return;

        mainPanel.connectionNote.ChangeText("Connect");

        mainPanel.playerPolaroids[0].Ready(false);
        mainPanel.playerPolaroids[1].Ready(false);
        mainPanel.isReady = false;
        mainPanel.readyNote.Disable();
        mainPanel.connectionNote.Enable();
        mainPanel.hostNote.Enable();
        mainPanel.quitNote.Enable();
        mainPanel.settingsMenu.isConnected = false;
        mainPanel.SetOpponentPolaroid();

    }

    public void StartGame()
    {
        game.LoadGame();
    }

    public void ChooseCharacter(Character character)
    {
        Debug.Log("Choose " + character.name + ".");
        chosenCharacter = character.directory;
        CmdChooseCharacter(chosenCharacter);
    }

    [Command]
    private void CmdChooseCharacter(string characterDirectory)
    {
        gameManager.PlayerChooseCharacter(playerIdx, characterDirectory);
    }

    public void Accuse(Character character)
    {
        Debug.Log("Accused " + character.name + ".");
        accusedCharacter = character.directory;
        CmdAccuse(character.directory);
    }

    [Command]
    private void CmdAccuse(string characterDirectory)
    {
        gameManager.Accuse(characterDirectory);
    }

    public void FinishedTurn()
    {
        Debug.Log("Finished turn.");

        if (accusedCharacter == string.Empty)
            CmdFinishedTurn(false);
        else
            CmdFinishedTurn(true);
    }

    [Command]
    private void CmdFinishedTurn(bool hasAccused)
    {
        gameManager.FinishedTurn(hasAccused);
    }

    public void StopGame()
    {
        if (!isHost)
        {
            NetworkManager.singleton.StopClient();
            Debug.Log("Disconnected from server.");
        }

        game.StopGame();
    }
}
