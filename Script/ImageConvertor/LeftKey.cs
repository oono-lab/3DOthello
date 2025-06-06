using UnityEngine;
using UnityEngine.UI;

public class LeftKey : MonoBehaviour
{
    public Sprite pressedSprite;

    private Image targetImage;
    private Sprite normalSprite;

    void Start()
    {
        targetImage = this.GetComponent<Image>();

        if (targetImage != null)
        {
            normalSprite = targetImage.sprite;
        }
        else
        {
            Debug.LogError("Image コンポーネントが見つかりません。");
        }
    }

    void Update()
    {
        if (targetImage == null) return;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            targetImage.sprite = pressedSprite;
        }
        else
        {
            targetImage.sprite = normalSprite;
        }
    }
}
