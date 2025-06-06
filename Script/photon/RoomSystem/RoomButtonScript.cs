using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class RoomButtonScript : MonoBehaviour
{
    public TextMeshProUGUI roomNameText;
    private string roomName;

    public void SetRoom(string name)
    {
        roomName = name;
        roomNameText.text = name;
    }

    public void OnClickJoin()
    {
        PhotonNetwork.JoinRoom(roomNameText.text);
    }

}
