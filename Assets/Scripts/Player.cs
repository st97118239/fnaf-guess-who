using Mirror;
using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : NetworkBehaviour
{
    [SyncVar] public string chosenCharacter;
    [SyncVar] public int playerIdx;
    [SyncVar] public bool isReadyToPlay;
    [SyncVar] public string accusedCharacter;

    public Game game;
    public MainPanel mainPanel;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gameManager.NewPlayer(this);

        if (isLocalPlayer)
        {
            game = FindFirstObjectByType<Game>();
            mainPanel = game.mainPanel;
            game.player = this;
            gameManager.player = this;

            if (gameManager.player1 != this)
                gameManager.opponent = gameManager.player1;

            Debug.Log("Player object connected.");
        }
        else if (!isServer)
        {
            if (gameManager.player1 == gameManager.player)
            {
                gameManager.opponent = gameManager.player2;
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

    public void Disconnect()
    {
        gameManager.player = null;
        gameManager.opponent = null;

        Invoke(nameof(RemoveConnection), 0.5f);
    }

    public void ForceDisconnect()
    {
        gameManager.player = null;
        gameManager.opponent = null;

        Invoke(nameof(RpcRemoveConnection), 0.2f);
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

    private void RemoveConnection()
    {
        NetworkManager.singleton.StopClient();
        Debug.Log("Disconnected from server.");

        mainPanel.connectionNote.ChangeText("Connect");

        mainPanel.connectionNote.Enable();
        mainPanel.hostNote.Enable();
    }

    [ClientRpc]
    private void RpcRemoveConnection()
    {
        NetworkManager.singleton.StopClient();
        Debug.Log("Disconnected from server.");

        mainPanel.connectionNote.ChangeText("Connect");

        mainPanel.connectionNote.Enable();
        mainPanel.hostNote.Enable();
    }

    public void StartGame()
    {
        game.StartGame();
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
        if (playerIdx == 1)
            gameManager.player1ChosenCharacter = characterDirectory;
        else if (playerIdx == 2)
            gameManager.player2ChosenCharacter = characterDirectory;

        Debug.Log("Player " + playerIdx + " has chosen " + characterDirectory);
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
        game.StopGame();
    }
}
