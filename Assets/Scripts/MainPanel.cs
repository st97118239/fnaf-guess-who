using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : NetworkBehaviour
{
    public Game game;

    public Note readyNote;
    public Note connectionNote;
    public Note hostNote;
    public Note listCreatorNote;
    public Note quitNote;

    public void ReadyUp()
    {
        if (game.player.isLocalPlayer && !game.player.isReadyToPlay)
        {
            game.player.isReadyToPlay = true;
            Debug.Log("Player is ready.");
            readyNote.Disable();
        }
    }

    public void Host()
    {
        if (NetworkClient.active && isServer)
        {
            Debug.Log("Stopping host.");

            game.gameManager.DisconnectAll();

            Invoke(nameof(HostStopDelay), 1);
        }
        else if (!NetworkClient.active)
        {
            connectionNote.Disable();

            NetworkManager.singleton.StartHost();
            Debug.Log("Starting host.");
            hostNote.ChangeText("Stop host");
        }
    }

    private void HostStopDelay()
    {
        NetworkManager.singleton.StopHost();

        connectionNote.Disable();

        hostNote.ChangeText("Start host");
    }

    public void Connect()
    {
        if (NetworkClient.isConnected)
        {
            Debug.Log("Disconnecting from server.");

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
    }

    public void ClientConnected()
    {
        connectionNote.ChangeText("Disconnect");
        connectionNote.Enable();
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
