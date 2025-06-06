using Photon.Pun;
using UnityEngine;

public class SynchronousProcessing : MonoBehaviourPunCallbacks
{
    public GameObject onlinesystem;

    void Start()
    {
        photonView.RPC("SyncSettingToAll", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber, OthelloScript.FIELD_SIZE, OthelloScript.positional_complement, OthelloScript.isHard, OthelloScript.isCustum);
    }

    [PunRPC]
    void SyncSettingToAll(int senderActorNumber, int FIELD_SIZE, float positional_complement, bool isHard, bool isCustum)
    {
        // 設定を同期
        OthelloScript.FIELD_SIZE = FIELD_SIZE;
        OthelloScript.positional_complement = positional_complement;
        OthelloScript.isHard = isHard;
        OthelloScript.isCustum = isCustum;

        // 設定を受け取ったら送信者に完了通知
        PhotonView senderView = PhotonView.Find(senderActorNumber); // ActorNumberでは見つからないので別の手段で送信
        photonView.RPC("OnSettingSyncComplete", PhotonPlayerFromActorNumber(senderActorNumber));
    }

    [PunRPC]
    void OnSettingSyncComplete()
    {
        // 設定が完了したことを確認して表示
        onlinesystem.SetActive(true);
    }

    // 補助: ActorNumber から PhotonPlayer を取得
    Photon.Realtime.Player PhotonPlayerFromActorNumber(int actorNumber)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == actorNumber)
                return player;
        }
        return null;
    }
}
