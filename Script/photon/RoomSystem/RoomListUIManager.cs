using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomListUIManager : MonoBehaviourPunCallbacks
{
    #region UI参照
    // ルームボタンのプレハブ
    public GameObject roomButtonPrefab;

    // ボタンを並べる親パネル (Vertical Layout Group付き)
    public Transform roomListPanel;

    // 表示中の部屋数を表示するテキスト
    public TextMeshProUGUI RoomListCount;
    #endregion

    #region 内部管理用
    // 現在表示中のルームボタンを管理する辞書
    private Dictionary<string, GameObject> roomButtons = new Dictionary<string, GameObject>();
    #endregion

    #region 初期化処理
    void Start()
    {
        // ロビーに参加
        PhotonNetwork.JoinLobby();

        // シーンの自動同期を有効に
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log("Auto:" + PhotonNetwork.AutomaticallySyncScene);
    }
    #endregion

    #region ルームリスト更新処理
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("RoomList:" + roomList);

        int maxRooms = 10;
        int shownCount = 0;

        foreach (RoomInfo info in roomList)
        {
            if (shownCount >= maxRooms)
                break;

            // 削除された部屋
            if (info.RemovedFromList)
            {
                if (roomButtons.ContainsKey(info.Name))
                {
                    Destroy(roomButtons[info.Name]);
                    roomButtons.Remove(info.Name);
                }
                continue;
            }

            // 新しい部屋、またはまだ表示されていない部屋
            if (!roomButtons.ContainsKey(info.Name))
            {
                GameObject newButton = Instantiate(roomButtonPrefab, roomListPanel.transform);
                RoomButtonScript btnScript = newButton.GetComponent<RoomButtonScript>();
                btnScript.SetRoom(info.Name);
                roomButtons.Add(info.Name, newButton);
                shownCount++;
            }
        }

        // 表示中の部屋数を TextMeshPro に表示
        RoomListCount.text = $"{roomButtons.Count} / {maxRooms}";
    }
    #endregion
}
