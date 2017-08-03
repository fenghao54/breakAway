using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    
    // 游戏开始玩家的血量
    public int startingHealth = 100;
    // 玩家当前血量
    public int currentHealth;
    // The time in seconds after we last took damage before we can be damaged again.
    //在玩家再次受到伤害之前最后受到伤害之后，这段时间不会受到伤害
    public float invulnerabilityTime = 1f;
    //多长时间无敌
    // The time in seconds before the background healthbar goes down after we last took damage.
    public float timeAfterWeLastTookDamage = 1f;
   
    //UI显示的血量是绿色
    public Slider healthSliderForeground;
    
    //UI显示的血量是红色
    public Slider healthSliderBackground;
   
    //伤害
    public Image damageImage;
    //玩家死亡播放的音乐
   
    public AudioClip deathClip;
    // 颜色褪色取样值，在插值的时候用
    public float flashSpeed = 5f;
    // 闪光颜色
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    public Text healthText; //生命值数值显示UI

    //更新动画组件
    Animator anim;
    // 音乐组件
    AudioSource playerAudio;
    // 涉及玩家移动
    PlayerMovement playerMovement;
    // 涉及玩家设计
    PlayerShooting playerShooting;
    // 玩家是否死亡
    bool isDead;
    // 玩家正在遭受攻击
    bool damaged;
    // The damage accumulated for the current time frame.
    float timer;
    //在被攻击的时候用，给玩家蒙皮加一个颜色，在游戏中被攻击的时候，会变红的效果。
    SkinnedMeshRenderer myRenderer;
    Color rimColor;

    void Awake()
    {
        // 赋值，引用
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponentInChildren<PlayerShooting>();
       
        // 初始化玩家血量
        currentHealth = startingHealth;

        // Get the Player Skinned Mesh Renderer.活动玩家的蒙皮网格
        SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        //遍历所有的蒙皮网格，如果游戏对象是Player,就赋值给myRenderer
        foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
        {
            if (meshRenderer.gameObject.name == "Player")
            {
                myRenderer = meshRenderer;
                break;
            }
        }
    }

    void Start()
    {
        //rimColor = myRenderer.materials[0].GetColor("_RimColor");//这句话不是太明白
    }

    void Update()
    {
        // 如果受到攻击
        if (damaged)
        {
            // 闪光颜色
            damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            // 如果没有受到伤害了，颜色清除
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;

        // If the timer exceeds the time between attacks, the player is in range and this enemy is alive attack.
        //如果大于无敌时间，并且在怪物攻击范围内，就会遭受攻击
        if (timer >= timeAfterWeLastTookDamage)
        {
            //背景颜色一点一点变化成前景颜色，插值。
            healthSliderBackground.value = Mathf.Lerp(healthSliderBackground.value, healthSliderForeground.value, 2 * Time.deltaTime);
        }

        //显示生命值
        healthText.text = currentHealth.ToString();

        // 关掉被攻击开关
        damaged = false;
    }


    public void TakeDamage(int amount)
    {
        if (timer < invulnerabilityTime)
        {
            return;
        }
        //停止协同，停止名字为“Isheit”的协同动作
        StopCoroutine("Ishit");
        StartCoroutine("Ishit");

        // 设置正在遭受攻击，让屏幕闪烁
        damaged = true;

        // 当前生命值减去受到的攻击伤害值
        currentHealth -= amount;

        if (currentHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }

        // 把前景滑块设置为当前生命值
        healthSliderForeground.value = currentHealth;

        // Accumulate damage.
        //累计伤害
        timer = 0;

        // 播放被打的声效
        playerAudio.Play();

        // 如果当前血量低于0就标记死亡
        if (currentHealth <= 0 && !isDead)
        {
           
            Death();
        }
    }

    IEnumerator Ishit()
    {
        //这一部应该是给玩家模型加一个颜色在被攻击的时候
        Color newColor = new Color(10, 0, 0, 0);

        myRenderer.materials[0].SetColor("_RimColor", newColor);

        float time = 1;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            if (elapsedTime < (time / 2))
            {
                newColor = Color.Lerp(newColor, rimColor, elapsedTime / time);
            }
            myRenderer.materials[0].SetColor("_RimColor", newColor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void AddHealth(int amount)
    {
        //加血
        currentHealth += amount;

        if (currentHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }

        //设置血量
        healthSliderForeground.value = currentHealth;
    }


    void Death()
    {
        // 标记玩家已经死亡
        isDead = true;
        
        // 关闭残留的设计效果
        playerShooting.DisableEffects();

        // 播放死亡动画
        anim.SetTrigger("Die");

        // 播放死亡音效
        playerAudio.clip = deathClip;
        playerAudio.Play();

        // 不能攻击/不能设计
        playerMovement.enabled = false;
        playerShooting.enabled = false;
    }
}