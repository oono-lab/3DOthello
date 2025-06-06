using UnityEngine;
using UnityEngine.UI;

public class DisableButtonOnClick : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(DisableButton);
    }

    void DisableButton()
    {
        button.interactable = false;
    }
}
