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
        if (gameManager.player1 == conn.identity.gameObject.GetComponent<Player>())
        {
            Debug.Log("Player 1 disconnected.");
            gameManager.PlayerDisconnected(1);
        }
        else if (gameManager.player2 == conn.identity.gameObject.GetComponent<Player>())
        {
            Debug.Log("Player 2 disconnected.");
            gameManager.PlayerDisconnected(2);
        }
        else
        {
            Debug.Log("Unkown Player disconnected.");
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        Debug.Log("Client disconnected.");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        Debug.Log("Server disconnected.");
    }
}
