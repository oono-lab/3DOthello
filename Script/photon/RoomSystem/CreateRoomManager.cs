using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class CreateRoomManager : MonoBehaviourPunCallbacks
{
    private bool _shouldCreateRoom;
    public OnlineOthelloScript othelloScript;
    public Image fadeImage;
    public float fadeDuration = 1f;
    public GameObject CreateRoomPanel;
    //public Material[] materials;
    public AlphaValueChange[] alphaValueChanges;
    private string currentUIState = "InitialUI";
    private TextMeshProUGUI roomCreatedMessageText;
    private int OnlineBattleScene = 9;
    private HashSet<int> playersSynced = new HashSet<int>();

    void Start()
    {
        roomCreatedMessageText = CreateRoomPanel.GetComponentInChildren<TextMeshProUGUI>();
    }
    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void OnCreateRoomButtonClicked()
    {   
        // まだ接続されていなければ、接続してフラグだけ立てる
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            _shouldCreateRoom = true;
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Photon...");
            return;
        }

        // 接続済みなら即ルーム作成
        CreateRandomRoom();
    }

    private void CreateRandomRoom()
    { 
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
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("新しいプレイヤーがルームに参加しました: " + PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
           
            Debug.Log("ルームのプレイヤーが2人になりました。設定を同期します...");
            // まず設定同期のRPCを送信
            photonView.RPC("SyncSettingToAll", RpcTarget.Others,
                PhotonNetwork.LocalPlayer.ActorNumber,  // 送信元 ActorNumber を渡す
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
        // 受け取った設定を適用
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
        // 設定同期が完了したクライアントの ActorNumber を追加
        int actorNumber = info.Sender.ActorNumber;
        playersSynced.Add(actorNumber);

        Debug.Log($"設定同期完了通知を受信: {info.Sender.NickName}");

        // 全員の同期が完了したらシーン遷移（マスタークライアントのみが処理）
        if (PhotonNetwork.IsMasterClient && playersSynced.Count == PhotonNetwork.CurrentRoom.PlayerCount - 1)
        {
            Debug.Log("全クライアントが設定同期完了。フィールド状態を送信してシーンをロードします...");
            for (int i = 0; i < alphaValueChanges.Length; i++)
            {
                alphaValueChanges[i].OnlineColorMatch();
            }
            othelloScript.SendFieldStateToParticipantsCustum();
            othelloScript.SendFieldStateToParticipantsFieldState();
            currentUIState = GetCurrentUIState();
            PhotonNetwork.LoadLevel(OnlineBattleScene);
            //photonView.RPC("SyncUIState", RpcTarget.Others, currentUIState);
            photonView.RPC("RPC_SyncLoadLevel", RpcTarget.OthersBuffered);
        }
    }
    private string GetCurrentUIState()
    {
        // ここで現在のUI状態を取得する処理を記述
        // 例: 現在のUIボタンの状態など
        return "InitialUI";  // 例: 初期状態として "InitialUI" を返す
    }

    // マスタークライアントからUI状態を同期
    [PunRPC]
    private void SyncUIState(string uiState)
    {
        Debug.Log("マスタークライアントからUI状態を受信しました: " + uiState);

        // 受信したUI状態に基づいて画面を更新
        UpdateUI(uiState);
    }

    // UIを更新する処理（例としてstringを使用）
    private void UpdateUI(string uiState)
    {
        Debug.Log("UI状態を更新: " + uiState);

        // ここでUI状態を更新する処理を記述
        // 例: 受信したUI状態に応じてボタンの表示やアクションを変更
    }
    // ActorNumber から Player を取得
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

}
