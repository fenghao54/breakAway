using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    //�ӵ��ٶ� 
    public float speed = 600.0f;
    //�ӵ���ը���Ͱ뾶
    public float explosionForce=10000f;
    public float explosionRadius=100f;

    //�ӵ�����ʱ��Ϊ3��
    public float life = 3;
    //��ͨ������Ч��
    public ParticleSystem normalTrailParticles;
    //����������Ч��
    public ParticleSystem bounceTrailParticles;
    //��͸����Ч��
    public ParticleSystem pierceTrailParticles;
    //�������Ч��
    public ParticleSystem ImpactParticles;

    public int damage = 20;

    //�Ƿ�Ϊ��͸��
    public bool piercing = false;
    //�Ƿ�Ϊ������
    public bool bounce = false;

    //�ӵ���ɫ
    public Color bulletColor;
    public AudioClip bounceSound;
    public AudioClip hitSound;

    //�ٶ�
    Vector3 velocity;
    //����
    Vector3 force;
    //��λ�� 
    Vector3 newPos;
    //��λ��
    Vector3 oldPos;
    //����
    Vector3 direction;

    //�Ƿ����Ŀ��
    bool hasHit = false;

    RaycastHit lastHit;

    // Reference to the audio source.
    AudioSource bulletAudio;

    float timer;

    void Awake()
    {
        bulletAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        newPos = transform.position;
        oldPos = newPos;

        // Set our particle colors.
        normalTrailParticles.startColor = bulletColor;
        bounceTrailParticles.startColor = bulletColor;
        pierceTrailParticles.startColor = bulletColor;
        ImpactParticles.startColor = bulletColor;

        //һ��ʼ��ʱ��ʹ��Ĭ�ϵ�����ϵͳ
        normalTrailParticles.gameObject.SetActive(true);

        //�����ӵ��Ļ�������Ϊ1���ٶ�Ϊ20
        if (bounce)
        {
            bounceTrailParticles.gameObject.SetActive(true);
            normalTrailParticles.gameObject.SetActive(false);
            life = 1;
            speed = 20;
        }

        //��͸���Ļ����ٶ�Ϊ40
        if (piercing)
        {
            pierceTrailParticles.gameObject.SetActive(true);
            normalTrailParticles.gameObject.SetActive(false);

            speed = 40;
        }
    }

    void Update()
    {
        if (hasHit)
        {
            return;
        }

        // ��ʼ��ʱ
        timer += Time.deltaTime;

        // ��ʼ�����ӵ�
        if (timer >= life)
        {
            Dissipate();
        }

        //��ǰ���λ���ƶ���Y���ϲ���Ҫ�����ٶ�.
        velocity = transform.forward;
        velocity.y = 0;
        //�ٶȺͷ����趨����
        velocity = velocity.normalized * speed;

        // �ӵ���λ��
        newPos += velocity * Time.deltaTime;

        // ����ӵ�·�����ǲ�����ײ��ʲô����
        direction = newPos - oldPos;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            RaycastHit[] hits = Physics.RaycastAll(oldPos, direction, distance);


            // �ҵ���һ�����õ���ײ��
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                if (ShouldIgnoreHit(hit))
                {
                    //������ѭ��
                    continue;
                }

                // ֪ͨ��ײ
                OnHit(hit);

                lastHit = hit;

                if (hasHit)
                {
                    // �������е�ѭ��
                    newPos = hit.point;
                    break;
                }
            }
        }

        oldPos = transform.position;
        transform.position = newPos;
    }

    /*
     * So we don't hit the same enemy twice with the same raycast when we have
     * piercing shots. The shot can still bounce on a wall, come back and hit
     * the enemy again if we have both bouncing and piercing shots.
     */
    bool ShouldIgnoreHit(RaycastHit hit)
    {
        if (lastHit.point == hit.point || lastHit.collider == hit.collider)
            return true;

        return false;
    }

    /**
     * �ӵ���ײ����Ϸ���󽫷���������
     */
    void OnHit(RaycastHit hit)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        if (hit.transform.tag == "Environment")
        {
            newPos = hit.point;
            ImpactParticles.transform.position = hit.point;
            ImpactParticles.transform.rotation = rotation;
            ImpactParticles.Play();

            if (bounce)
            {
                Vector3 reflect = Vector3.Reflect(direction, hit.normal);
                transform.forward = reflect;
                bulletAudio.clip = bounceSound;
                bulletAudio.pitch = Random.Range(0.8f, 1.2f);
                bulletAudio.Play();
            }
            else
            {
                hasHit = true;
                bulletAudio.clip = hitSound;
                bulletAudio.volume = 0.5f;
                bulletAudio.pitch = Random.Range(1.2f, 1.3f);
                bulletAudio.Play();
                DelayedDestroy();
            }
        }

        if (hit.transform.tag == "Enemy")
        {
            ImpactParticles.transform.position = hit.point;
            ImpactParticles.transform.rotation = rotation;
            ImpactParticles.Play();
            //���򵽵��˵�ʱ�򣬵��˺��ˡ�
            hit.rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            // Try and find an EnemyHealth script on the gameobject hit.
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
           

           

            // If the EnemyHealth component exist...
            if (enemyHealth != null)
            {
                // ... the enemy should take damage.
                enemyHealth.TakeDamage(damage, hit.point);
            }

            if (!piercing)
            {
                hasHit = true;
                DelayedDestroy();
            }

            bulletAudio.clip = hitSound;
            bulletAudio.volume = 0.5f;
            bulletAudio.pitch = Random.Range(1.2f, 1.3f);
            bulletAudio.Play();
        }
    }

    // Just a method for destroying the game object, but which
    // first detaches the particle effect and leaves it for a
    // second. Called if the bullet end its life in midair
    // so we get an effect of the bullet fading out instead
    // of disappearing immediately.
    //�ӵ���ɢ��Ч��
    void Dissipate()
    {
        normalTrailParticles.enableEmission = false;
        normalTrailParticles.transform.parent = null;
        Destroy(normalTrailParticles.gameObject, normalTrailParticles.duration);

        if (bounce)
        {
            bounceTrailParticles.enableEmission = false;
            bounceTrailParticles.transform.parent = null;
            Destroy(bounceTrailParticles.gameObject, bounceTrailParticles.duration);
        }

        if (piercing)
        {
            pierceTrailParticles.enableEmission = false;
            pierceTrailParticles.transform.parent = null;
            Destroy(pierceTrailParticles.gameObject, pierceTrailParticles.duration);
        }

        Destroy(gameObject);
    }


    /// <summary>
    /// �ӳ�����
    /// </summary>
    void DelayedDestroy()
    {
        normalTrailParticles.gameObject.SetActive(false);
        if (bounce)
        {
            bounceTrailParticles.gameObject.SetActive(false);
        }
        if (piercing)
        {
            pierceTrailParticles.gameObject.SetActive(false);
        }
        Destroy(gameObject, hitSound.length);
    }
}