using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActiveBool : MonoBehaviour
{
    public GameObject ActiveObj;
   public void GameObjectActive(bool hantei)
    {
        ActiveObj.SetActive(hantei);
    }
}
