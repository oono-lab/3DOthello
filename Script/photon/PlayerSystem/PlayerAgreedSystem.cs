using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgreedSystem : MonoBehaviourPun
{
    private bool localPlayerAgreed = false;  // �����̓��ӃX�e�[�^�X
    private bool remotePlayerAgreed = false; // ����v���C���[�̓��ӃX�e�[�^�X
    private int OnlineBattleScene = 9;
    public GameObject BattleMatchWaite;
    public void OnPlayerAgreed()
    {
        localPlayerAgreed = true;

        // ������hantei�X�e�[�^�X�𑊎�v���C���[�ɒʒm
        photonView.RPC("RPCHantei", RpcTarget.Others);
        BattleMatchWaite.SetActive(true);
        if (localPlayerAgreed && remotePlayerAgreed) { PhotonNetwork.LoadLevel(OnlineBattleScene); }
    }
    [PunRPC]
    void RPCHantei()
    {
        remotePlayerAgreed = true;
        if (localPlayerAgreed && remotePlayerAgreed) PhotonNetwork.LoadLevel(OnlineBattleScene);
    }
}
