using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlphaValueChange : MonoBehaviourPunCallbacks
{   
    public Material TargetObj;     
    public TextMeshProUGUI currentValueText;
    public UnityEngine.UI.Scrollbar scrollbar;
    public string variableKey = "";
    // Start is called before the first frame update
    void Start()
    {   
        scrollbar.onValueChanged.AddListener(delegate { onValueChangedAlpha(); });
    }
    void Update()
    {
        Color sourceColor = TargetObj.color;
        sourceColor.a = scrollbar.value;
        currentValueText.text = scrollbar.value.ToString("F1");
    }
    public void onValueChangedAlpha() {
        
        PlayerPrefs.SetFloat(variableKey, scrollbar.value);
        PlayerPrefs.Save();
        
    }


    public void OnlineColorMatch()
    {
        photonView.RPC("OnlineColorMatchPun", RpcTarget.Others, TargetObj.name);
    }
    [PunRPC]
    private void OnlineColorMatchPun(string materialName)
    {
        Material found = Resources.Load<Material>("Materials/" + materialName);
        TargetObj = found;
    }
}
