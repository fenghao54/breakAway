using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{

    /// <summary>
    /// 子弹颜色
    /// </summary>
    public Color[] bulletColors;

    Animator animorShoot;
    public int animationLayer=0;
    bool isShoot = false;
    bool canShoot = false;
    public float timeShoot=0.1f;
    /// <summary>
    /// 反弹周期
    /// </summary>
    public float bounceDuration = 10;

    /// <summary>
    /// 穿透时间
    /// </summary>
    public float pierceDuration = 10;


    // 子弹伤害
    public int damagePerShot = 20;

    //子弹散弹个数
    public int numberOfBullets = 1;

    //子弹总数
    [SerializeField]
    private int totalBullets = 100;

    //携带弹药最大量
    int maxBulletsAmount = 200;


    // 每次射击间隔的时间
    public float timeBetweenBullets = 0.15f;

    //子弹角度
    public float angleBetweenBullets = 10f;


    // 射击的范围
    public float range = 100f;
    //射击层标记
    public LayerMask shootableMask;
    // 反弹子弹的UI
    public Image bounceImage;
    // 穿透子弹的UI
    public Image pierceImage;
    //子弹数显示UI
    public Text bulletsAmount;

    public GameObject bullet;

    /// <summary>
    /// 子弹生成锚点
    /// </summary>
    public Transform bulletSpawnAnchor;

    // 开火的计时器
    float timer;
    // 射击射线
    Ray shootRay;
    // 击中点
    RaycastHit shootHit;
    // 枪的粒子效果
    ParticleSystem gunParticles;
    // 枪线
    LineRenderer gunLine;
    // 开枪音效
    AudioSource gunAudio;
    // 发光体
    Light gunLight;
    // 呈现时间
    float effectsDisplayTime = 0.2f;
    float bounceTimer;
    float pierceTimer;
    bool bounce;
    bool piercing;
    Color bulletColor;

    private bool skillIsStart1 = false;//技能1释放标识
    public float skillColdTime1 = 2f; //Z 技能1冷却时间
    public Image skillFillImage1;//技能1填充图
    private float skilltimer = 0f;//技能计时器

    //技能1触发检测
    private void Skill1Trigger()
    {
        if (skillIsStart1 == true)
        {
            skilltimer += Time.deltaTime;
            skillFillImage1.GetComponent<Image>().fillAmount = (skillColdTime1 - skilltimer) / skillColdTime1;
            if (skilltimer >= skillColdTime1)//技能冷却结束
            {
                skillFillImage1.GetComponent<Image>().fillAmount = 0;
                skilltimer = 0;
                skillIsStart1 = false;
            }

        }
    }
    //反弹时间
    public float BounceTimer
    {
        get { return bounceTimer; }
        set { bounceTimer = value; }
    }
    //穿透时间
    public float PierceTimer
    {
        get { return pierceTimer; }
        set { pierceTimer = value; }
    }

    void Awake()
    {
        animorShoot = GetComponentInParent<Animator>();
        gunParticles = GetComponent<ParticleSystem>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();

        bounceTimer = bounceDuration;
        pierceTimer = pierceDuration;
        if (animorShoot)
        {
            animorShoot.SetLayerWeight(animationLayer, 1);
        }
    }

    void Update()
    {
        //每一帧都检查弹药量是否越界
        BulletLimit();
        //如果反弹时间在反弹弹周期内，当前就是反弹的弹，超过时间，就灭有反弹弹了
        if (bounceTimer < bounceDuration)
        {
            bounce = true;
        }
        else
        {
            bounce = false;
        }
        //穿透弹，解释同上
        if (pierceTimer < pierceDuration)
        {
            piercing = true;
        }
        else
        {
            piercing = false;
        }

        bulletColor = bulletColors[0];
        if (bounce)
        {
            //子弹颜色设置
            bulletColor = bulletColors[1];
            //子弹UI图片设置
            bounceImage.color = bulletColors[1];
        }
        bounceImage.gameObject.SetActive(bounce);

        if (piercing)
        {
            //同上
            bulletColor = bulletColors[2];
            //同上
            pierceImage.color = bulletColors[2];
        }
        pierceImage.gameObject.SetActive(piercing);

        if (piercing & bounce)//如果又是穿透弹，又是反弹弹（不知道是不是这样）
        {
            bulletColor = bulletColors[3];
            bounceImage.color = bulletColors[3];
            pierceImage.color = bulletColors[3];
        }
        //枪的粒子颜色
        gunParticles.startColor = bulletColor;
        gunLight.color = bulletColor;

        // 每一帧之后，都会增加每种子弹的持续时间，之后如果时间超出预先设置的子弹周期，该子弹就失效了
        bounceTimer += Time.deltaTime;
        pierceTimer += Time.deltaTime;
        timer += Time.deltaTime;

        // 如果按下鼠标左键，还有子弹的话就调用射击的函数
        if (Input.GetButtonDown("Fire1") )
        {
            isShoot = true;
            animorShoot.SetBool("Fire", isShoot);
            canShoot = true;
        }
        if (Input.GetButton("Fire1"))
        {
            timeShoot -= Time.deltaTime;
            if (timeShoot < 0&& canShoot==true)
            {
                Shoot();
                canShoot = false;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            isShoot = false;
            timeShoot = 0.1f;
            animorShoot.SetBool("Fire", isShoot);
        }

        //如果时间超出子弹周期，该子弹就失效了（摧毁）
        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
          
            DisableEffects();
        }

        //Z 技能1按键触发
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            skillIsStart1 = true;
        }

        Skill1Trigger();//技能释放检测

        //更新UI子弹数
        bulletsAmount.text = totalBullets.ToString();
    }

    public void DisableEffects()
    {
        // 发光体不能使用
        gunLight.enabled = false;
    }

    void  Shoot()
    {
       
        //重新设置子弹摧毁时间
        timer = 0f;
        

        // 设置不同子弹发出的声音
        gunAudio.pitch = Random.Range(1.2f, 1.3f);

        if (bounce)
        {
            //音效的最高音
            gunAudio.pitch = Random.Range(1.1f, 1.2f);
        }

        if (piercing)
        {
            gunAudio.pitch = Random.Range(1.0f, 1.1f);
        }

        if (piercing & bounce)
        {
            gunAudio.pitch = Random.Range(0.9f, 1.0f);
        }
        gunAudio.Play();

        // Enable the light.
        //设置子弹发光体的强度
        gunLight.intensity = 1 + (0.5f * (numberOfBullets - 1));
        gunLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles.
        //发光体的粒子会在发光体射出之后再播放，有延迟的效果
        gunParticles.Stop();
        gunParticles.startSize = 1 + (0.1f * (numberOfBullets - 1));
        gunParticles.Play();

        // 设置射线开始位置和方向
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        for (int i = 0; i < numberOfBullets; i++)
        {
            // 使子弹均匀分布
            //确定子弹生成的时候的方向
            float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2) * (numberOfBullets - 1));
            Quaternion rot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
            GameObject instantiatedBullet = Instantiate(bullet, bulletSpawnAnchor.transform.position, rot) as GameObject;
            instantiatedBullet.GetComponent<Bullet>().piercing = piercing;
            instantiatedBullet.GetComponent<Bullet>().bounce = bounce;
            instantiatedBullet.GetComponent<Bullet>().bulletColor = bulletColor;
        }

        totalBullets -= numberOfBullets; //每射击一次子弹数减少
       
    }

    /// <summary>
    /// 检查弹药量是否越界
    /// </summary>
    void BulletLimit()
    {
        if (totalBullets >= 200)
        {
            totalBullets = maxBulletsAmount;
        }
    }

    /// <summary>
    /// 当掉落物品为AddBullet时，增加弹药量
    /// </summary>
    /// <param name="amount"></param>
    public void AddBullet(int amount)
    {
        totalBullets += amount;
    }
}