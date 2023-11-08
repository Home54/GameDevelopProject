using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class sceneswitch : MonoBehaviour
{
    private static sceneswitch instance;
    public static void showme()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(true);
        }
    }
    public static void hideme()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(false);
        }
    }

    public Button button1; // 将按钮1分配到这个字段
    public Button button2; // 将按钮2分配到这个字段
    public Button button3; // 将按钮3分配到这个字段
    public Button button4;
    public GameObject canvas1; // 指向第一个 Canvas
    public GameObject canvas2; // 指向第二个 Canvas

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 添加按钮点击事件监听器
        button1.onClick.AddListener(OnButton1Click);
        button2.onClick.AddListener(OnButton2Click);
       
    }

    private void OnButton1Click()
    {
        // 切换到场景1（填写你的场景名称）
        //canvas1.SetActive(false);
        canvas2.SetActive(true);
    }

    private void OnButton2Click()
    {
        // 切换到场景2（填写你的场景名称）
        canvas2.SetActive(false);
    }

    
}
