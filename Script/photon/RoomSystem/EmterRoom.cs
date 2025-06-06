using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmterRoom : MonoBehaviourPunCallbacks
{
    public OnlineOthelloScript othelloScript;

    // Start is called before the first frame update
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
    }


}
