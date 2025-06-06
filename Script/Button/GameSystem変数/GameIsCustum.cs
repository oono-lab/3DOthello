using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIsCustum : MonoBehaviour
{
    public GameObject OthelloScriptObj;
    private OthelloScript othelloScript;
    // Start is called before the first frame update
    void Start()
    {
        othelloScript = OthelloScriptObj.GetComponent<OthelloScript>();
    }
    // Start is called before the first frame update
    public void IsCustum(bool hantei)
    {

        OthelloScript.isCustum = hantei;
    }
}
