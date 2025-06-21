using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class CreateRoomManager : MonoBehaviourPunCallbacks
{
    #region 変数宣言・UI参照
    private bool _shouldCreateRoom;
    public OnlineOthelloScript othelloScript;
    public Image fadeImage;
    public float fadeDuration = 1f;
    public GameObject CreateRoomPanel;
    public AlphaValueChange[] alphaValueChanges;

    private string currentUIState = "InitialUI";
    private TextMeshProUGUI roomCreatedMessageText;
    private int OnlineBattleScene = 9;

    // 設定同期が完了したプレイヤーのActorNumberを保持
    private HashSet<int> playersSynced = new HashSet<int>();
    #endregion

    #region Unityイベント
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        roomCreatedMessageText = CreateRoomPanel.GetComponentInChildren<TextMeshProUGUI>();
    }
    #endregion

    #region UIボタンイベント
    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnCreateRoomButtonClicked()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            _shouldCreateRoom = true;
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Photon...");
            return;
        }

        CreateRandomRoom();
    }
    #endregion

    #region ルーム作成処理
    private void CreateRandomRoom()
    {
        int maxRooms = 10;

        if (PhotonNetwork.CountOfRooms >= maxRooms)
        {
            Debug.Log("ルーム数が満杯です。作成をキャンセルします。");
            roomCreatedMessageText.text = "ルーム数が満杯のため作成できませんでした。時間を置いてから再度作成してください。";
            return;
        }

        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2
        };

        string roomName = "Room" + Random.Range(1000, 9999);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"Creating room: {roomName}");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master.");
        if (_shouldCreateRoom)
        {
            _shouldCreateRoom = false;
            CreateRandomRoom();
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully");
        roomCreatedMessageText.text = $"あなたは {PhotonNetwork.CurrentRoom.Name} を作成しました。プレイヤーが来るまでしばらくお待ちください。";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Room creation failed: {message}");
    }
    #endregion

    #region ルーム参加と同期処理
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("新しいプレイヤーがルームに参加しました: " + PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ルームのプレイヤーが2人になりました。設定を同期します...");
            photonView.RPC("SyncSettingToAll", RpcTarget.Others,
                PhotonNetwork.LocalPlayer.ActorNumber,
                OthelloScript.FIELD_SIZE,
                OthelloScript.positional_complement,
                OthelloScript.isHard,
                OthelloScript.isCustum
            );
        }
    }

    [PunRPC]
    void SyncSettingToAll(int senderActorNumber, int FIELD_SIZE, float positional_complement, bool isHard, bool isCustum)
    {
        // 設定を適用
        OthelloScript.FIELD_SIZE = FIELD_SIZE;
        OthelloScript.positional_complement = positional_complement;
        OthelloScript.isHard = isHard;
        OthelloScript.isCustum = isCustum;

        Debug.Log("設定を受信・適用しました。送信者に完了通知を返します。");

        Player sender = GetPlayerFromActorNumber(senderActorNumber);
        if (sender != null)
        {
            photonView.RPC("OnSettingSyncComplete", sender);
        }
    }

    [PunRPC]
    void OnSettingSyncComplete(PhotonMessageInfo info)
    {
        int actorNumber = info.Sender.ActorNumber;
        playersSynced.Add(actorNumber);

        Debug.Log($"設定同期完了通知を受信: {info.Sender.NickName}");

        if (PhotonNetwork.IsMasterClient && playersSynced.Count == PhotonNetwork.CurrentRoom.PlayerCount - 1)
        {
            Debug.Log("全クライアントが設定同期完了。フィールド状態を送信してシーンをロードします...");

            foreach (var alpha in alphaValueChanges)
            {
                alpha.OnlineColorMatch();
            }

            othelloScript.SendFieldStateToParticipantsCustum();
            othelloScript.SendFieldStateToParticipantsFieldState();

            currentUIState = GetCurrentUIState();
            PhotonNetwork.LoadLevel(OnlineBattleScene);
            photonView.RPC("RPC_SyncLoadLevel", RpcTarget.OthersBuffered);
        }
    }
    #endregion

    #region UI同期処理
    private string GetCurrentUIState()
    {
        return "InitialUI";
    }

    [PunRPC]
    private void SyncUIState(string uiState)
    {
        Debug.Log("マスタークライアントからUI状態を受信しました: " + uiState);
        UpdateUI(uiState);
    }

    private void UpdateUI(string uiState)
    {
        Debug.Log("UI状態を更新: " + uiState);
        // 必要に応じてUIを制御
    }
    #endregion

    #region ユーティリティ
    Player GetPlayerFromActorNumber(int actorNumber)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == actorNumber)
                return player;
        }
        return null;
    }

    [PunRPC]
    void RPC_SyncLoadLevel()
    {
        Debug.Log("[SLAVE] RPC_SyncLoadLevel を受信したので、同じシーンをロードします。");
        PhotonNetwork.LoadLevel(OnlineBattleScene);
    }
    #endregion
}
