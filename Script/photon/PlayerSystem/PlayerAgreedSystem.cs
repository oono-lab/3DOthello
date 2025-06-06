using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgreedSystem : MonoBehaviourPun
{
    private bool localPlayerAgreed = false;  // 自分の同意ステータス
    private bool remotePlayerAgreed = false; // 相手プレイヤーの同意ステータス
    private int OnlineBattleScene = 9;
    public GameObject BattleMatchWaite;
    public void OnPlayerAgreed()
    {
        localPlayerAgreed = true;

        // 自分のhanteiステータスを相手プレイヤーに通知
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
