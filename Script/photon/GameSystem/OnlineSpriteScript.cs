using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class OnlineSpriteScript : MonoBehaviour
{
    public AlphaValueChange[] alphaValueChange;

    public OnlineOthelloScript othelloScript;
    public Material WallObjMaterial;
    public Material WhitePoint;
    public Material BlackPoint;
    public Material NoneObjMaterial;
    public Material ChoicePoint;
    public GameObject[] Alpfalist;
    private GameObject WhiteObj;
    private GameObject BlackObj;
    private GameObject ChoiceObj;
    private GameObject NoneObj;
    private GameObject WallObjClone;
    private int blackIndex = 0;
    private int whiteIndex = 1;
    private int noneIndex = 2;
    private int noneChoiceIndex = 3;
    private int wallObjIndex = 4;
    private bool Whitehantei = false;
    private bool Blackhantei = false;
    private bool WallHantei = false;
    private bool ChoiceHantei = false;
    private bool NoneHanantei = false;
    public void SetState(OnlineOthelloScript.SpriteState state)
    {



        Renderer objectRenderer = GetComponent<Renderer>();
        if (state == OnlineOthelloScript.SpriteState.White)
        {
            #region 白マス状態のオブジェクトを表示する。
            if (WallHantei)
            {
                gameObject.GetComponent<Renderer>().enabled = true;
                foreach (Transform child in gameObject.transform) child.gameObject.SetActive(false);
                WallHantei = false;

            }
            else
            {
                Color sourceColor = WhitePoint.color;
                sourceColor.a = alphaValueChange[whiteIndex].scrollbar.value;
                objectRenderer.material.color = sourceColor;
                EnableTransparency(objectRenderer.material);
            }
            #endregion

        }
        else if (state == OnlineOthelloScript.SpriteState.Black)
        {
            #region 黒マス状態のオブジェクトを表示する。
            if (WallHantei)
            {
                gameObject.GetComponent<Renderer>().enabled = true;
                foreach (Transform child in gameObject.transform) child.gameObject.SetActive(false);
                WallHantei = false;


            }
            else
            {
                Color sourceColor = BlackPoint.color;
                sourceColor.a = alphaValueChange[blackIndex].scrollbar.value;
                objectRenderer.material.color = sourceColor;
                EnableTransparency(objectRenderer.material);
            }
            #endregion
        }
        else if (state == OnlineOthelloScript.SpriteState.Wall)
        {
            #region 何も置けない状態のオブジェクトを表示する。
            gameObject.GetComponent<Renderer>().enabled = false;
            foreach (Transform child in gameObject.transform)
            {
                Renderer objectRendererwall = child.GetComponent<Renderer>();
                child.gameObject.SetActive(true);
                Color sourceColor = WallObjMaterial.color;
                sourceColor.a = alphaValueChange[wallObjIndex].scrollbar.value;
                objectRendererwall.material.color = sourceColor;
                EnableTransparency(objectRendererwall.material);
            }
            WallHantei = true;
            #endregion
        }
        else if (state == OnlineOthelloScript.SpriteState.NoneChoice)
        {
            #region 置けるマス状態のオブジェクトを表示する。
            Color sourceColor = ChoicePoint.color;
            sourceColor.a = alphaValueChange[noneChoiceIndex].scrollbar.value;
            objectRenderer.material.color = sourceColor;
            EnableTransparency(objectRenderer.material);
            #endregion
        }
        else if (state == OnlineOthelloScript.SpriteState.None)
        {
            if (WallHantei)
            {
                gameObject.GetComponent<Renderer>().enabled = true;
                foreach (Transform child in gameObject.transform) child.gameObject.SetActive(false);
                WallHantei = false;

            }
            else
            {
                Color sourceColor = NoneObjMaterial.color;
                sourceColor.a = alphaValueChange[noneIndex].scrollbar.value;
                objectRenderer.material.color = sourceColor;
                EnableTransparency(objectRenderer.material);
            }

        }

    }
    #region 透明度を変更。
    private void EnableTransparency(Material material)
    {
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
    #endregion
}
