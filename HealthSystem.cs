using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("生命值设置")]
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private int currentHealth;

    [Header("毒雾效果设置")]
    [SerializeField] private float poisonDamageInterval = 1f; // 每1秒伤害一次
    [SerializeField] private Color poisonColor = new Color(0.5f, 0f, 0.5f); // 紫色
    [SerializeField] private float colorChangeSpeed = 2f; // 颜色变化速度

    [Header("UI引用")]
    [SerializeField] private GameObject[] healthIcons; // 心形图标数组
    [SerializeField] private GameObject deathPanel; // 死亡面板

    [Header("重生设置")]
    [SerializeField] private Transform spawnPoint; // 重生点

    // 组件引用
    private Renderer characterRenderer;
    private Color originalColor;
    private bool isInPoisonFog = false;
    private float poisonTimer = 0f;
    private CharacterController characterController;

    void Start()
    {
        // 初始化生命值
        currentHealth = maxHealth;

        // 获取渲染器组件
        characterRenderer = GetComponent<Renderer>();
        if (characterRenderer != null)
        {
            originalColor = characterRenderer.material.color;
        }

        // 获取角色控制器
        characterController = GetComponent<CharacterController>();

        // 确保死亡面板初始隐藏
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        // 更新UI
        UpdateHealthUI();
    }

    void Update()
    {
        // 处理毒雾效果
        if (isInPoisonFog)
        {
            // 增加计时器
            poisonTimer += Time.deltaTime;

            // 每秒扣血一次
            if (poisonTimer >= poisonDamageInterval)
            {
                TakeDamage(1);
                poisonTimer = 0f;
            }

            // 逐渐变为紫色
            if (characterRenderer != null)
            {
                characterRenderer.material.color = Color.Lerp(
                    characterRenderer.material.color,
                    poisonColor,
                    Time.deltaTime * colorChangeSpeed
                );
            }
        }
        else if (characterRenderer != null && characterRenderer.material.color != originalColor)
        {
            // 离开毒雾后，逐渐恢复原来的颜色
            characterRenderer.material.color = Color.Lerp(
                characterRenderer.material.color,
                originalColor,
                Time.deltaTime * colorChangeSpeed
            );
        }
    }

    // 进入毒雾区域
    public void EnterPoisonFog()
    {
        isInPoisonFog = true;
        poisonTimer = 0f; // 重置计时器
    }

    // 离开毒雾区域
    public void ExitPoisonFog()
    {
        isInPoisonFog = false;
    }

    // 受到伤害
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // 已经死亡，不再受伤

        currentHealth -= damage;

        // 更新UI
        UpdateHealthUI();

        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 恢复生命值
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    // 更新生命值UI
    private void UpdateHealthUI()
    {
        if (healthIcons == null || healthIcons.Length == 0) return;

        // 显示或隐藏对应数量的心形图标
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (healthIcons[i] != null)
            {
                healthIcons[i].SetActive(i < currentHealth);
            }
        }
    }

    // 角色死亡
    private void Die()
    {
        // 显示死亡UI
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        // 禁用角色控制
        BeanController beanController = GetComponent<BeanController>();
        if (beanController != null)
        {
            beanController.enabled = false;
        }

        // 如果有角色控制器，禁用它
        if (characterController != null)
        {
            characterController.enabled = false;
        }
    }

    // 重生角色
    // 重生角色
    public void Respawn()
    {
        Debug.Log("执行重生函数");

        // 恢复生命值
        currentHealth = maxHealth;
        UpdateHealthUI();

        // 隐藏死亡UI
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
            Debug.Log("隐藏死亡面板");
        }
        else
        {
            Debug.LogError("无法隐藏死亡面板：引用为空!");
        }

        // 尝试多种方式获取重生位置
        Transform respawnPosition = null;

        // 方法1：使用静态引用
        if (SpawnPointManager.spawnPointTransform != null)
        {
            respawnPosition = SpawnPointManager.spawnPointTransform;
            Debug.Log("使用SpawnPointManager获取重生位置");
        }
        // 方法2：使用本地引用
        else if (spawnPoint != null)
        {
            respawnPosition = spawnPoint;
            Debug.Log("使用本地引用获取重生位置");
        }
        // 方法3：尝试通过名称查找
        else
        {
            GameObject spawnObj = GameObject.Find("SpawnPoint");
            if (spawnObj != null)
            {
                respawnPosition = spawnObj.transform;
                // 更新本地引用以便下次使用
                spawnPoint = respawnPosition;
                Debug.Log("通过名称查找获取重生位置");
            }
        }

        // 如果找到了重生位置，重置角色位置
        if (respawnPosition != null)
        {
            Debug.Log("将角色位置重置到: " + respawnPosition.position);

            // 如果有CharacterController，需要先禁用它再移动位置，然后重新启用
            if (characterController != null)
            {
                // 确保禁用CharacterController
                characterController.enabled = false;

                // 设置位置
                transform.position = respawnPosition.position;

                // 重新启用CharacterController
                characterController.enabled = true;
                Debug.Log("已重置角色位置并重新启用CharacterController");
            }
            else
            {
                transform.position = respawnPosition.position;
                Debug.Log("已重置角色位置（无CharacterController）");
            }
        }
        else
        {
            Debug.LogError("无法找到任何重生点！请检查场景中是否存在SpawnPoint对象。");
        }

        // 后续代码保持不变...

        // 获取并重置BeanController
        BeanController beanController = GetComponent<BeanController>();
        if (beanController != null)
        {
            // 重新启用BeanController
            beanController.enabled = true;

            // 调用新增的重置方法
            beanController.ResetController();

            // 启用控制
            beanController.SetControlEnabled(true);

            Debug.Log("已重置并启用BeanController");
        }
        else
        {
            Debug.LogWarning("找不到BeanController组件!");
        }

        // 确保游戏对象处于激活状态
        gameObject.SetActive(true);

        // 恢复原来的颜色
        if (characterRenderer != null)
        {
            characterRenderer.material.color = originalColor;
        }

        // 确保我们不在毒雾中
        isInPoisonFog = false;
    }
}