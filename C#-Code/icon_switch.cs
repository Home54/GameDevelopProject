using UnityEngine;
using UnityEngine.UI;

public class icon_switch : MonoBehaviour
{
    public Image buttonImage; // 这里将按钮的Image组件拖放到这个字段
    public Sprite normalIcon; // 正常状态下的图标
    public Sprite pressedIcon; // 按下状态下的图标

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (buttonImage.sprite == normalIcon)
        {
            buttonImage.sprite = pressedIcon;
        }
        else
        {
            buttonImage.sprite = normalIcon;
        }
    }
}
