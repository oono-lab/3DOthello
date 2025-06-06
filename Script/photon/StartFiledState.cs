using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFiledState : MonoBehaviour
{
    public OnlineOthelloScript othelloScript;
    void Awake()
    {   
        othelloScript.StartFiledState();
    }

   
}
