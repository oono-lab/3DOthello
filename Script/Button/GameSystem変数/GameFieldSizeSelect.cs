using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldSizeSelect : MonoBehaviour
{
    public GameObject OthelloScriptObj;
    private OthelloScript othelloScript;
    void Start()
    {
        othelloScript = OthelloScriptObj.GetComponent<OthelloScript>();
    }
    // Start is called before the first frame update
    public void FieldSizeSelect(int size)
    {
        
        OthelloScript.FIELD_SIZE = size;
    }
}
