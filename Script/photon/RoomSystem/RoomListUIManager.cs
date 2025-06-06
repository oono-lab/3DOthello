using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomListUIManager : MonoBehaviourPunCallbacks
{
    public GameObject roomButtonPrefab; // �v���n�u
    public Transform roomListPanel;     // �p�l�� (Vertical Layout Group�t��)

    private Dictionary<string, GameObject> roomButtons = new Dictionary<string, GameObject>();
    void Start()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Auto:"+PhotonNetwork.AutomaticallySyncScene);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("RoomList:"+ roomList);
        foreach (RoomInfo info in roomList)
        {
            // �폜���ꂽ����
            if (info.RemovedFromList)
            {
                if (roomButtons.ContainsKey(info.Name))
                {
                    Destroy(roomButtons[info.Name]);
                    roomButtons.Remove(info.Name);
                }
                continue;
            }

            // �V�K�܂��͍X�V
            if (!roomButtons.ContainsKey(info.Name))
            {
                GameObject newButton = Instantiate(roomButtonPrefab, roomListPanel.transform);
                RoomButtonScript btnScript = newButton.GetComponent<RoomButtonScript>();
                btnScript.SetRoom(info.Name);
                roomButtons.Add(info.Name, newButton);
            }
        }
    }
}
