using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlphaValueChange : MonoBehaviourPunCallbacks
{
    #region Public Variables - Inspector参照用
    // 透過値を変更する対象のマテリアル
    public Material TargetObj;

    // 現在の値を表示するテキスト
    public TextMeshProUGUI currentValueText;

    // 値を調整するスクロールバー
    public UnityEngine.UI.Scrollbar scrollbar;

    // 保存するPlayerPrefsのキー
    public string variableKey = "";
    #endregion

    #region Unityイベントメソッド処理
    void Start()
    {
        // スクロールバーの値が変わったときのイベント登録
        scrollbar.onValueChanged.AddListener(delegate { onValueChangedAlpha(); });
    }

    void Update()
    {
        // アルファ値をスクロールバーに応じて毎フレーム更新
        Color sourceColor = TargetObj.color;
        sourceColor.a = scrollbar.value;
        TargetObj.color = sourceColor;  // 忘れずに更新！

        // テキスト表示も更新
        currentValueText.text = scrollbar.value.ToString("F1");
    }
    #endregion

    #region アルファ変更時の保存処理
    public void onValueChangedAlpha()
    {
        // プレイヤー設定としてアルファ値を保存
        PlayerPrefs.SetFloat(variableKey, scrollbar.value);
        PlayerPrefs.Save();
    }
    #endregion

    #region オンライン同期処理
    // 他のプレイヤーにマテリアル名を送信して同期させる
    public void OnlineColorMatch()
    {
        photonView.RPC("OnlineColorMatchPun", RpcTarget.Others, TargetObj.name);
    }

    // RPCで受け取ったマテリアル名をもとに Resources から読み込み設定
    [PunRPC]
    private void OnlineColorMatchPun(string materialName)
    {
        Material found = Resources.Load<Material>("Materials/" + materialName);
        TargetObj = found;
    }
    #endregion
}
