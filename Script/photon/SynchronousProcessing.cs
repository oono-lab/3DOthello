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
        // �ݒ�𓯊�
        OthelloScript.FIELD_SIZE = FIELD_SIZE;
        OthelloScript.positional_complement = positional_complement;
        OthelloScript.isHard = isHard;
        OthelloScript.isCustum = isCustum;

        // �ݒ���󂯎�����瑗�M�҂Ɋ����ʒm
        PhotonView senderView = PhotonView.Find(senderActorNumber); // ActorNumber�ł͌�����Ȃ��̂ŕʂ̎�i�ő��M
        photonView.RPC("OnSettingSyncComplete", PhotonPlayerFromActorNumber(senderActorNumber));
    }

    [PunRPC]
    void OnSettingSyncComplete()
    {
        // �ݒ肪�����������Ƃ��m�F���ĕ\��
        onlinesystem.SetActive(true);
    }

    // �⏕: ActorNumber ���� PhotonPlayer ���擾
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
