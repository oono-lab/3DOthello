using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectEscapeboo : MonoBehaviour
{
    public GameObject MonoBehaviourObject;
    public GameObject[] TargetObjectList;
    public GameObject[] TargetObjectList1;
    
    private bool isHantei=false;

    void Start()
    {
        isHantei = false;
    }
    // Update is called once per frame
    void Update()
    {
 
            if (Input.GetKeyDown(KeyCode.Escape)&& isHantei == false)
            {
                ActiveMonoBehaviour();
            }

    }
    public void ActiveMonoBehaviour() {
        isHantei = !isHantei;
        MonoBehaviour[] scripts = MonoBehaviourObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) script.enabled = !isHantei;

        foreach (GameObject Object in TargetObjectList) Object.gameObject.SetActive(isHantei);
        foreach (GameObject Object in TargetObjectList1) Object.gameObject.SetActive(!isHantei);
    }
}
