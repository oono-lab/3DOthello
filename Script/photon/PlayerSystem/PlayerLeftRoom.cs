using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerLeftRoom : MonoBehaviourPunCallbacks
{
    public GameObjectEscapeboo escapeboo;
    public GameObject disconectRoom;
    // Start is called before the first frame update
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("プレイヤーがルームを退出しました: " + otherPlayer.NickName);

        // もしルーム内に1人しか残っていなければ、そのプレイヤーも強制退出させる
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            foreach (GameObject Object in escapeboo.TargetObjectList1) Object.gameObject.SetActive(false);
            escapeboo.gameObject.SetActive(false);

            disconectRoom.SetActive(true);

        }
    }
}
