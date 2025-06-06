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
        // �܂��ڑ�����Ă��Ȃ���΁A�ڑ����ăt���O�������Ă�
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            _shouldCreateRoom = true;
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Photon...");
            return;
        }

        // �ڑ��ς݂Ȃ瑦���[���쐬
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
        roomCreatedMessageText.text = $"���Ȃ��� {PhotonNetwork.CurrentRoom.Name} ���쐬���܂����B�v���C���[������܂ł��΂炭���҂����������B";
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
        Debug.Log("�V�����v���C���[�����[���ɎQ�����܂���: " + PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
           
            Debug.Log("���[���̃v���C���[��2�l�ɂȂ�܂����B�ݒ�𓯊����܂�...");
            // �܂��ݒ蓯����RPC�𑗐M
            photonView.RPC("SyncSettingToAll", RpcTarget.Others,
                PhotonNetwork.LocalPlayer.ActorNumber,  // ���M�� ActorNumber ��n��
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
        // �󂯎�����ݒ��K�p
        OthelloScript.FIELD_SIZE = FIELD_SIZE;
        OthelloScript.positional_complement = positional_complement;
        OthelloScript.isHard = isHard;
        OthelloScript.isCustum = isCustum;

        Debug.Log("�ݒ����M�E�K�p���܂����B���M�҂Ɋ����ʒm��Ԃ��܂��B");
        

            Player sender = GetPlayerFromActorNumber(senderActorNumber);
        if (sender != null)
        {
            photonView.RPC("OnSettingSyncComplete", sender);
        }
    }

    [PunRPC]
    void OnSettingSyncComplete(PhotonMessageInfo info)
    {
        // �ݒ蓯�������������N���C�A���g�� ActorNumber ��ǉ�
        int actorNumber = info.Sender.ActorNumber;
        playersSynced.Add(actorNumber);

        Debug.Log($"�ݒ蓯�������ʒm����M: {info.Sender.NickName}");

        // �S���̓���������������V�[���J�ځi�}�X�^�[�N���C�A���g�݂̂������j
        if (PhotonNetwork.IsMasterClient && playersSynced.Count == PhotonNetwork.CurrentRoom.PlayerCount - 1)
        {
            Debug.Log("�S�N���C�A���g���ݒ蓯�������B�t�B�[���h��Ԃ𑗐M���ăV�[�������[�h���܂�...");
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
        // �����Ō��݂�UI��Ԃ��擾���鏈�����L�q
        // ��: ���݂�UI�{�^���̏�ԂȂ�
        return "InitialUI";  // ��: ������ԂƂ��� "InitialUI" ��Ԃ�
    }

    // �}�X�^�[�N���C�A���g����UI��Ԃ𓯊�
    [PunRPC]
    private void SyncUIState(string uiState)
    {
        Debug.Log("�}�X�^�[�N���C�A���g����UI��Ԃ���M���܂���: " + uiState);

        // ��M����UI��ԂɊ�Â��ĉ�ʂ��X�V
        UpdateUI(uiState);
    }

    // UI���X�V���鏈���i��Ƃ���string���g�p�j
    private void UpdateUI(string uiState)
    {
        Debug.Log("UI��Ԃ��X�V: " + uiState);

        // ������UI��Ԃ��X�V���鏈�����L�q
        // ��: ��M����UI��Ԃɉ����ă{�^���̕\����A�N�V������ύX
    }
    // ActorNumber ���� Player ���擾
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
        Debug.Log("[SLAVE] RPC_SyncLoadLevel ����M�����̂ŁA�����V�[�������[�h���܂��B");
        PhotonNetwork.LoadLevel(OnlineBattleScene);
    }

}
