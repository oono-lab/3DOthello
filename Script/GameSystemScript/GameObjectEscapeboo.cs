using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectEscapeboo : MonoBehaviour
{
    #region Inspector設定項目
    // 無効化・有効化対象の親オブジェクト（スクリプトがアタッチされている）
    public GameObject MonoBehaviourObject;

    // 表示するUIやGameObject
    public GameObject[] TargetObjectList;

    // 非表示にするUIやGameObject
    public GameObject[] TargetObjectList1;
    #endregion

    #region 内部状態
    // UIが開いているかどうかを判定するフラグ
    private bool isHantei = false;
    #endregion

    #region Unityイベントメソッド
    void Start()
    {
        isHantei = false;

        // 起動時はUIを開いた状態にして操作無効化
        ActiveMonoBehaviour();
    }

    void Update()
    {
        // Escapeキーが押されたら UI 開閉処理
        if (Input.GetKeyDown(KeyCode.Escape) && isHantei == false)
        {
            ActiveMonoBehaviour();
        }
    }
    #endregion

    #region 表示・制御切り替え処理
    public void ActiveMonoBehaviour()
    {
        // 状態をトグル
        isHantei = !isHantei;

        // 指定された GameObject に付属する MonoBehaviour をすべて有効／無効切り替え
        MonoBehaviour[] scripts = MonoBehaviourObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
            script.enabled = !isHantei;

        // UI表示を切り替え
        foreach (GameObject obj in TargetObjectList)
            obj.SetActive(isHantei);

        foreach (GameObject obj in TargetObjectList1)
            obj.SetActive(!isHantei);
    }
    #endregion
}
