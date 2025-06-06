using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustumSelectNumber : MonoBehaviour
{
    public OthelloScript othelloScript;
    public OnlineOthelloScript onlineOthelloScript;
    public GameObject[] ObjectActiveNumber;
    private int MaxListValue = 3;
    private int MinListValue = 0;
    // Start is called before the first frame update

    // Update is called once per frame
    public void CustumSelectButtonChangeDown()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (othelloScript.CustumSelectBotton == MinListValue)
        {
            othelloScript.CustumSelectBotton = MaxListValue;
            ObjectActiveNumber[MinListValue].SetActive(false);
            ObjectActiveNumber[othelloScript.CustumSelectBotton].SetActive(true);
        }
        else 
        {
            othelloScript.CustumSelectBotton--;
            ObjectActiveNumber[othelloScript.CustumSelectBotton+1].SetActive(false);
            ObjectActiveNumber[othelloScript.CustumSelectBotton].SetActive(true);
        }
        //Debug.Log(othelloScript.CustumSelectBotton);

    }
    public void CustumSelectButtonChangeUp()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (othelloScript.CustumSelectBotton == MaxListValue)
        {
            othelloScript.CustumSelectBotton = MinListValue;
            ObjectActiveNumber[MaxListValue].SetActive(false);
            ObjectActiveNumber[othelloScript.CustumSelectBotton].SetActive(true);
        }
        else
        {
            othelloScript.CustumSelectBotton++;
            ObjectActiveNumber[othelloScript.CustumSelectBotton - 1].SetActive(false);
            ObjectActiveNumber[othelloScript.CustumSelectBotton].SetActive(true);
        }
    }
    public void CustumSelectButtonChangeDownOnline()
        {
            EventSystem.current.SetSelectedGameObject(null);
            if (onlineOthelloScript.CustumSelectBotton == MinListValue)
            {
                onlineOthelloScript.CustumSelectBotton = MaxListValue;
                ObjectActiveNumber[MinListValue].SetActive(false);
                ObjectActiveNumber[onlineOthelloScript.CustumSelectBotton].SetActive(true);
            }
            else
            {
                onlineOthelloScript.CustumSelectBotton--;
                ObjectActiveNumber[onlineOthelloScript.CustumSelectBotton + 1].SetActive(false);
                ObjectActiveNumber[onlineOthelloScript.CustumSelectBotton].SetActive(true);
            }
            //Debug.Log(othelloScript.CustumSelectBotton);

        }
        public void CustumSelectButtonChangeUpOnline()
        {
            EventSystem.current.SetSelectedGameObject(null);
            if (onlineOthelloScript.CustumSelectBotton == MaxListValue)
            {
                onlineOthelloScript.CustumSelectBotton = MinListValue;
                ObjectActiveNumber[MaxListValue].SetActive(false);
                ObjectActiveNumber[onlineOthelloScript.CustumSelectBotton].SetActive(true);
            }
            else
            {
                onlineOthelloScript.CustumSelectBotton++;
                ObjectActiveNumber[onlineOthelloScript.CustumSelectBotton - 1].SetActive(false);
                ObjectActiveNumber[onlineOthelloScript.CustumSelectBotton].SetActive(true);
            }

            //Debug.Log(othelloScript.CustumSelectBotton);
        }

}

