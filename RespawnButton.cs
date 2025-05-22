using UnityEngine;
using UnityEngine.UI;

public class RespawnButton : MonoBehaviour
{
    [SerializeField] private HealthSystem playerHealth;
    private Button buttonComponent;

    private void Start()
    {
        // 获取按钮组件并添加点击事件
        buttonComponent = GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(OnRespawnButtonClicked);
            Debug.Log("重生按钮初始化完成，已添加点击监听");
        }
        else
        {
            Debug.LogWarning("重生按钮缺少Button组件! 尝试手动添加点击处理...");

            // 如果对象上没有Button组件，可以尝试添加一个
            buttonComponent = gameObject.AddComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(OnRespawnButtonClicked);
                Debug.Log("已自动添加Button组件并设置点击监听");

                // 如果对象上没有图像组件，也添加一个基本的
                if (GetComponent<Image>() == null)
                {
                    Image image = gameObject.AddComponent<Image>();
                    image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // 深灰色，半透明
                }
            }
        }

        // 如果没有指定玩家的HealthSystem，尝试自动查找
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<HealthSystem>();
            if (playerHealth != null)
            {
                Debug.Log("自动找到玩家HealthSystem");
            }
            else
            {
                Debug.LogError("无法找到玩家HealthSystem，请手动设置引用!");
            }
        }
        else
        {
            Debug.Log("玩家HealthSystem已正确引用");
        }
    }

    // 此方法用于检测鼠标点击，作为备用方案
    private void Update()
    {
        // 如果鼠标左键被点击
        if (Input.GetMouseButtonDown(0))
        {
            // 检查鼠标是否在此对象上
            if (RectTransformUtility.RectangleContainsScreenPoint(
                GetComponent<RectTransform>(),
                Input.mousePosition,
                Camera.main))
            {
                Debug.Log("检测到按钮区域被点击");
                OnRespawnButtonClicked();
            }
        }

        // 使用R键作为备用重生方法
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("按下R键，调用重生功能");
            OnRespawnButtonClicked();
        }
    }

    private void OnRespawnButtonClicked()
    {
        Debug.Log("重生按钮被点击");
        if (playerHealth != null)
        {
            Debug.Log("调用玩家的Respawn方法");
            playerHealth.Respawn();
        }
        else
        {
            Debug.LogError("找不到玩家的HealthSystem组件!");
            // 尝试再次查找
            playerHealth = FindObjectOfType<HealthSystem>();
            if (playerHealth != null)
            {
                Debug.Log("重新找到玩家HealthSystem并调用Respawn");
                playerHealth.Respawn();
            }
        }
    }

    // 公共方法，可以通过Inspector调用
    public void TestRespawn()
    {
        OnRespawnButtonClicked();
    }
}