using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public int version;

    [SyncVar] public bool hasStarted;
    [SyncVar] public int round;
    [SyncVar] public int turn;
    [SyncVar] public bool needsToAccuse;
    [SyncVar] public bool hasFinished;

    public Player player1;
    public int player1Version;
    [SyncVar] public string[] player1List;
    [SerializeField] private string player1ChosenCharacter;
    [SerializeField] private string player1AccusedCharacter;
    [SyncVar] public bool player1Won;
    public Player player2;
    public int player2Version;
    [SyncVar] public string[] player2List;
    [SerializeField] private string player2ChosenCharacter;
    [SerializeField] private string player2AccusedCharacter;
    [SyncVar] public bool player2Won;

    public Player player;
    public Player opponent;

    private void OnEnable()
    {
        round = -1;
        turn = 0;

        if (isServer)
            Debug.Log("Started Server");
    }

    private void Update()
    {
        if (!isServer)
            return;

        if (round == -1 && turn == 0 && player1 && player2)
        {
            Debug.Log("Both players are connected.");
            turn = 1;
            Invoke(nameof(RpcEnableReady), 1f);
        }

        if (round == -1 && turn == 1 && player1.isReadyToPlay && player2.isReadyToPlay)
        {
            Debug.Log("Both players are ready. Loading game.");
            round = 0;
            turn = 0;
            hasStarted = true;
            Invoke(nameof(RpcStartGame), 0.5f);
        }

        if (round == 0 && player1ChosenCharacter != string.Empty && player2ChosenCharacter != string.Empty)
        {
            Debug.Log("Both players have chosen a character. Starting game.");
            StartRound();
        }

        if (round < 0 || hasFinished) 
            return;

        if (!player1 || !player2)
            DisconnectAll();
    }

    [ClientRpc]
    private void RpcEnableReady()
    {
        if (round == -1 && turn == 1 && player1 && player2)
            player.CanReady();
    }

    [ClientRpc]
    private void RpcDisableReady()
    {
        if (player)
            player.CanNotReady();
    }

    public void SetList(string[] givenArray)
    {
        CmdSetList(givenArray, player.playerIdx);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetList(string[] givenArray, int givenIndex)
    {
        switch (givenIndex)
        {
            case 1:
                player1List = givenArray;
                break;
            case 2:
                player2List = givenArray;
                break;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdVersionCheck(int pIdx, int givenVersion)
    {
        switch (pIdx)
        {
            case 1:
                player1Version = givenVersion;
                break;
            case 2:
                player2Version = givenVersion;
                break;
        }

        if (!player2)
            return;

        if (player1Version == 0 || player2Version == 0) 
            return;

        if (player1Version != player2Version)
            player2.RpcVersionDisconnect();
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPolaroidVariables()
    {
        RpcOpponentPolaroid();
    }

    [ClientRpc]
    public void RpcOpponentPolaroid()
    {
        player.mainPanel.SetOpponentPolaroid();
    }

    public void NewPlayer(Player newPlayer)
    {
        if (round != -1)
        {
            newPlayer.ForceDisconnect();
            return;
        }

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
            newPlayer.ForceDisconnect();
    }

    [ClientRpc]
    public void RpcStartGame()
    {
        player.StartGame();
    }

    public void StartRound()
    {
        round = 1;
        RpcStartTurn(Random.Range(1, 3), false);
    }

    [ClientRpc]
    public void RpcStartTurn(int playerTurn, bool hasToAccuse)
    {
        turn = playerTurn;
        Debug.Log(playerTurn + ", " + hasToAccuse);

        if (player.playerIdx == turn)
        {
            Debug.Log("It's now my turn.");
            player.game.StartRound(hasToAccuse);
        }
        else
        {
            Debug.Log("It's now the other player's turn.");
            player.game.characterSidebar.ChangeTurn(turn, hasToAccuse);
        }
    }

    public void FinishedTurn(bool hasAccused)
    {
        Debug.Log("Player " + turn + " finished their turn.");

        if (hasAccused)
        {
            switch (turn)
            {
                case 1:
                    Debug.Log("Player " + turn + " has accused " + player1AccusedCharacter);
                    break;
                case 2:
                    Debug.Log("Player " + turn + " has accused " + player2AccusedCharacter);
                    break;
            }

            if (needsToAccuse)
            {
                Debug.Log("Both players have accused a character.");
                FinishGame();
                return;
            }

            needsToAccuse = true;
            round = -100;

            switch (turn)
            {
                case 1:
                    turn = 2;
                    RpcStartTurn(2, true);
                    break;
                case 2:
                    turn = 1;
                    RpcStartTurn(1, true);
                    break;
            }
        }
        else
        {
            switch (turn)
            {
                case 1:
                    turn = 2;
                    RpcStartTurn(2, false);
                    break;
                case 2:
                    turn = 1;
                    round++;
                    RpcStartTurn(1, false);
                    break;
            }
        }
    }

    public void Accuse(string characterDirectory)
    {
        switch (turn)
        {
            case 1:
                player1AccusedCharacter = characterDirectory;
                break;
            case 2:
                player2AccusedCharacter = characterDirectory;
                break;
        }
    }

    public void FinishGame()
    {
        hasFinished = true;

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
        Invoke(nameof(ResetGame), 1.5f);
    }

    [ClientRpc]
    private void RpcFinishGame()
    {
        player.game.DetermineResult(player1Won, player2Won, opponent.chosenCharacter, player.accusedCharacter, player.chosenCharacter, opponent.accusedCharacter);
        player.StopGame();
    }

    public void ResetGame()
    {
        hasStarted = false;
        round = -1;
        turn = 0;
        needsToAccuse = false;
        hasFinished = false;
        player1 = null;
        player1Version = 0;
        player1List = null;
        player1ChosenCharacter = string.Empty;
        player1AccusedCharacter = string.Empty;
        player1Won = false;
        player2 = null;
        player2Version = 0;
        player2List = null;
        player2ChosenCharacter = string.Empty;
        player2AccusedCharacter = string.Empty;
        player2Won = false;

        player = null;
        opponent = null;
    }

    public void DisconnectAll()
    {
        if (player1)
            player1.RpcForceDisconnect();
        if (player2)
            player2.RpcForceDisconnect();
    }

    public void PlayerChooseCharacter(int pIdx, string charDir)
    {
        switch (pIdx)
        {
            case 1:
                player1ChosenCharacter = charDir;
                break;
            case 2:
                player2ChosenCharacter = charDir;
                break;
        }

        Debug.Log("Player " + pIdx + " has chosen " + charDir);
    }

    public void PlayerDisconnected(int playerToRemove)
    {
        round = -1;
        turn = 0;

        RpcDisableReady();
        RpcOpponentPolaroid();

        switch (hasFinished)
        {
            case false when hasStarted:
                DisconnectAll();
                break;
            case false when playerToRemove == 1 && player2 && !player1.isHost:
                player2.OpponentLeft();
                break;
            case false when playerToRemove == 2 && player1:
                player1.OpponentLeft();
                break;
        }

        switch (playerToRemove)
        {
            case 1:
                player1 = null;
                player1List = null;
                player1AccusedCharacter = string.Empty;
                player1ChosenCharacter = string.Empty;
                player1Won = false;
                break;
            case 2:
                player2 = null;
                player2List = null;
                player2AccusedCharacter = string.Empty;
                player2ChosenCharacter = string.Empty;
                player2Won = false;
                break;
        }
    }
}
