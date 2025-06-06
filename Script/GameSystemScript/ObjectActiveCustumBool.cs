using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectActiveCustumBool : MonoBehaviour
{
    public OthelloScript othelloScript;
    public GameObject OthelloScriptObj;
    public GameObject CustumObject;
    public GameObject GameObject;
    // Start is called before the first frame update
    void Start()
    {
        othelloScript = OthelloScriptObj.GetComponent<OthelloScript>();
        CustumObject.gameObject.SetActive(OthelloScript.isCustum);
        GameObject.gameObject.SetActive(!OthelloScript.isCustum);
    }
    public void isCustumBool()
    {
        if (OthelloScript.isCustum)
        {
            CustumObject.gameObject.SetActive(true);
            GameObject.gameObject.SetActive(false);
        }
        else {
            CustumObject.gameObject.SetActive(false);
            GameObject.gameObject.SetActive(true);
        }
        
    }
}
