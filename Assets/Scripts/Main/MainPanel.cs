using Mirror;
using UnityEngine;

public class MainPanel : NetworkBehaviour
{
    public Game game;

    public Note readyNote;
    public Note connectionNote;
    public Note hostNote;
    public Note listCreatorNote;
    public Note quitNote;

    public bool isHost;
    public bool isReady;

    public void ReadyUp()
    {
        if (game.player.isLocalPlayer && !game.player.isReadyToPlay)
        {
            game.player.isReadyToPlay = true;
            isReady = true;
            Debug.Log("Player is ready.");
            readyNote.Disable();
        }
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
            NetworkManager.singleton.StartHost();
            hostNote.ChangeText("Stop host");
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

            Invoke(nameof(CheckConnection), 1);
        }
    }

    public void CheckConnection()
    {
        if (NetworkClient.isConnected)
            return;

        Debug.Log("Can't connect.");
        NetworkManager.singleton.StopClient();

        hostNote.Enable();
        connectionNote.Enable();

        if (isReady)
            isReady = false;
    }

    public void ClientConnected()
    {
        connectionNote.ChangeText("Disconnect");
        if (!isHost)
            connectionNote.Enable();

        quitNote.Disable();
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

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
