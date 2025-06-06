using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyRebindingFromInputField : MonoBehaviour
{
    public UnityEngine.UI.InputField keyInputField;  
    public TextMeshProUGUI currentKeyText;
    public  KeyCode currentKey;
    void Start()
    {
        keyInputField.onValueChanged.AddListener(delegate { SetKeyFromInputField(); });
    }
    public void SetKeyFromInputField()
    {
        string input = keyInputField.text; 
        KeyCode newKey;

        
        if (System.Enum.TryParse(input, true, out newKey))
        {
            
            currentKey = newKey; 
            currentKeyText.text = $"Current Key: {currentKey}";
            
        }
    }

}
