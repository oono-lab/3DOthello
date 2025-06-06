using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComplementSelect : MonoBehaviour
{
    public GameObject OthelloScriptObj;
    private OthelloScript othelloScript;
    void Start()
    {
        othelloScript = OthelloScriptObj.GetComponent<OthelloScript>();
    }
    // Start is called before the first frame update
    public void ComplementSelect(float complement)
    {

        OthelloScript.positional_complement = complement;
    }
}
