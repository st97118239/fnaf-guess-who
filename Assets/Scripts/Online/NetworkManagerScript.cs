using Mirror;
using UnityEngine;

public class NetworkManagerScript : NetworkManager
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private MainPanel mainPanel;
    [SerializeField] private Game game;

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        Debug.Log("Client Connected.");

        mainPanel.ClientConnected();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Player player = conn.identity.gameObject.GetComponent<Player>();

        if (gameManager.player1 == player)
        {
            Debug.Log("Player 1 disconnected.");
            gameManager.PlayerDisconnected(1);
        }
        else if (gameManager.player2 == player)
        {
            Debug.Log("Player 2 disconnected.");
            gameManager.PlayerDisconnected(2);
        }
        else
        {
            Debug.Log("Unkown Player disconnected.");
        }

        if (player.isHost)
        {
            mainPanel.HostStop();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnClientDisconnect()
    {
        if (game.player && game.player.forcedLeave)
            game.player.ForceRemoveConnection();

        base.OnClientDisconnect();
        
        Debug.Log("Client disconnected.");
    }

    public override void OnStopServer()
    {
        bool p1 = false;
        bool p2 = false;

        if (gameManager.player1)
            p1 = true;
        if (gameManager.player2)
            p2 = true;

        if (p1 && p2)
        {
            if (!gameManager.player1.isHost)
                gameManager.player1.mainPanel.popupPaper.Show(Error.ServerDisconnected);
            else if (!gameManager.player2.isHost)
                gameManager.player2.mainPanel.popupPaper.Show(Error.ServerDisconnected);
        }

        base.OnStopServer();

        Debug.Log("Server stopped.");
    }
}
