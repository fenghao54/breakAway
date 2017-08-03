using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // 设置移动速度
    public float speed = 6f;

    // 移动方向
    Vector3 movement;
    // 动画接口
    Animator anim;
    // 刚体接口
    Rigidbody playerRigidbody;
   
    //确保怪物都在地面层，可以被射线射到。
    int floorMask;
    // 相机到屏幕中心的距离
    float camRayLength = 100f;
    //bool run = false;

    void Awake()
    {
        //创建一个地面层
        floorMask = LayerMask.GetMask("Floor");

        // 接口赋值
        anim = GetComponentInChildren<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
       
        //float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");


        //Move(h, v);

        Move(v);
        Turning();
       
        // Animate the player.
        
       
    }

    void Move(float h)
    {
       
       
           
        anim.SetBool("Run", h!=0?true:false);
        
        // 设置基础的移动
        //movement.Set(h, 0f, 0f);

        transform.position += transform.forward * h*speed * Time.deltaTime;
        //movement = movement.normalized * speed * Time.deltaTime;

        //向操控位置移动
        //playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning()
    {
        // 摄像机创造一个射线点，从相机到鼠标位置的
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 创建一个射线
        RaycastHit floorHit;

        // 如果射到地面
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            // 射到地面的坐标
            Vector3 playerToMouse = floorHit.point - transform.position;
            //只管平面的向量
            playerToMouse.y = 0f;
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            playerRigidbody.MoveRotation(newRotation);
        }
    }

 
    
}