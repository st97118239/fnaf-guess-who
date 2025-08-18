using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public bool hasStarted;

    public Player player1;
    public Player player2;

    public Player player;

    public void NewPlayer(Player newPlayer)
    {
        if (!player1)
        {
            player1 = newPlayer;
            player1.isP1 = true;
        }
        else if (!player2)
        {
            player2 = newPlayer;
            player2.isP1 = false;
        }
        else
        {
            if (newPlayer.isLocalPlayer)
                NetworkManager.singleton.StopClient();
        }

        if (newPlayer.isLocalPlayer)
            player = newPlayer;
    }
}
