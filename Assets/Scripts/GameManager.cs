using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SyncVar] public bool hasStarted;
    [SyncVar] public int round;
    [SyncVar] public int turn;
    [SyncVar] public bool needsToAccuse;

    public Player player1;
    public string player1ChosenCharacter;
    public string player1AccusedCharacter;
    public bool player1Won;
    public Player player2;
    public string player2ChosenCharacter;
    public string player2AccusedCharacter;
    public bool player2Won;

    public Player player;
    public Player opponent;

    private void Start()
    {
        if (isServer)
        {
            round = -1;
            Debug.Log("Started Server");
        }
        
    }

    private void Update()
    {
        if (!isServer)
            return;

        if (round == -1 && turn == 0 && player1 && player2)
        {
            Debug.Log("Both players are connected.");
            turn = 1;
            Invoke(nameof(RpcEnableReady), 0.5f);
            //RpcEnableReady();
        }

        if (round == -1 && turn == 1 && player1.isReadyToPlay && player2.isReadyToPlay)
        {
            Debug.Log("Both players are ready. Loading game.");
            round = 0;
            turn = 0;
            hasStarted = true;
            RpcStartGame();
        }

        if (round == 0 && player1ChosenCharacter != string.Empty && player2ChosenCharacter != string.Empty)
        {
            Debug.Log("Both players have chosen a character. Starting game.");
            round = 1;
            turn = 0;
        }

        if (round > 0 && turn == 0)
        {
            StartRound();
        }
    }

    [ClientRpc]
    private void RpcEnableReady()
    {
        if (round == -1 && turn == 1 && player1 && player2 && connectionToServer.identity)
            player.CanReady();
    }

    [ClientRpc]
    private void RpcDisableReady()
    {
        player.CanNotReady();
    }

    public void NewPlayer(Player newPlayer)
    {
        if (!player1)
        {
            player1 = newPlayer;
            player1.playerIdx = 1;
            player1.tag = "P1";
        }
        else if (!player2)
        {
            player2 = newPlayer;
            player2.playerIdx = 2;
            player2.tag = "P2";
        }
        else
            newPlayer.Disconnect();
    }

    [ClientRpc]
    public void RpcStartGame()
    {
        player.StartGame();
    }

    public void StartRound()
    {
        turn = 1;
        RpcStartTurn(1, false);
    }

    [ClientRpc]
    public void RpcStartTurn(int playerTurn, bool hasToAccuse)
    {
        turn = playerTurn;

        if (player.playerIdx == turn)
        {
            Debug.Log("It's now my turn.");
            player.game.StartRound(hasToAccuse);
        }
        else
        {
            Debug.Log("It's now the other player's turn.");
        }

        player.game.characterSidebar.ChangeTurn(turn, hasToAccuse);
    }

    public void FinishedTurn(bool hasAccused)
    {
        Debug.Log("Player " + turn + " finished their turn.");

        if (hasAccused)
        {
            if (turn == 1)
                Debug.Log("Player " + turn + " has accused " + player1AccusedCharacter);
            else if (turn == 2)
                Debug.Log("Player " + turn + " has accused " + player2AccusedCharacter);

            if (needsToAccuse)
            {
                Debug.Log("Both players have accused a character.");
                FinishGame();
                return;
            }

            needsToAccuse = true;
            round = -100;

            if (turn == 1)
            {
                turn = 2;
                RpcStartTurn(2, true);
            }
            else if (turn == 2)
            {
                turn = 1;
                RpcStartTurn(1, true);
            }
        }
        else
        {
            if (turn == 1)
            {
                turn = 2;
                RpcStartTurn(2, false);
            }
            else if (turn == 2)
            {
                turn = 1;
                RpcStartTurn(1, false);
            }
        }
    }

    public void Accuse(string characterDirectory)
    {
        if (turn == 1)
            player1AccusedCharacter = characterDirectory;
        else if (turn == 2)
            player2AccusedCharacter = characterDirectory;
    }

    public void FinishGame()
    {
        if (player1AccusedCharacter == player2ChosenCharacter)
        {
            Debug.Log("Player 1 accused Player 2's character: " + player2ChosenCharacter);
            player1Won = true;
        }
        else
            Debug.Log("Player 1 did not accuse Player 2's character. Player 1 accused " + player1AccusedCharacter + ", while Player 2 chose " + player2ChosenCharacter + ".");
        
        if (player2AccusedCharacter == player1ChosenCharacter)
        {
            Debug.Log("Player 2 accused Player 1's character: " + player1ChosenCharacter);
            player2Won = true;
        }
        else
            Debug.Log("Player 2 did not accuse Player 1's character. Player 2 accused " + player2AccusedCharacter + ", while Player 1 chose " + player1ChosenCharacter + ".");

        Invoke(nameof(RpcFinishGame), 1);
        Invoke(nameof(ResetGame), 2);
    }

    [ClientRpc]
    private void RpcFinishGame()
    {
        player.game.DetermineResult(player1Won, player2Won, opponent.chosenCharacter, player.accusedCharacter, player.chosenCharacter, opponent.accusedCharacter);
        player.StopGame();
    }

    private void ResetGame()
    {
        hasStarted = false;
        round = -1;
        turn = 0;
        needsToAccuse = false;
        player1 = null;
        player1ChosenCharacter = string.Empty;
        player1AccusedCharacter = string.Empty;
        player1Won = false;
        player2 = null;
        player2ChosenCharacter = string.Empty;
        player2AccusedCharacter = string.Empty;
        player2Won = false;
    }

    public void DisconnectAll()
    {
        player1.ForceDisconnect();
        player2.ForceDisconnect();

        round = -1;
        turn = -1;

        player1 = null;
        player1AccusedCharacter = string.Empty;
        player1ChosenCharacter = string.Empty;
        player1Won = false;
        player2 = null;
        player2AccusedCharacter = string.Empty;
        player2ChosenCharacter = string.Empty;
        player2Won = false;
    }

    public void PlayerDisconnected(int playerToRemove)
    {
        round = -1;
        turn = 0;

        if (playerToRemove == 1)
        {
            player1 = null;
            player1AccusedCharacter = string.Empty;
            player1ChosenCharacter = string.Empty;
            player1Won = false;
        }
        else if (playerToRemove == 2)
        {
            player2 = null;
            player2AccusedCharacter = string.Empty;
            player2ChosenCharacter = string.Empty;
            player2Won = false;
        }

        RpcDisableReady();
    }
}
