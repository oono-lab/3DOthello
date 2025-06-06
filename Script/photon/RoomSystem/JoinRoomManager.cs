using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class JoinRoomManager : MonoBehaviourPunCallbacks
{
    public void OnJoinRoomButtonClicked(string roomName)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Photon...");
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room");

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Join room failed: " + message);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master. Ready to join room.");
    }
}
