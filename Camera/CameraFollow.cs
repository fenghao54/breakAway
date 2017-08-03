using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	
    //相机跟随的目标位置
	public Transform target; 
	//跟随的速度，也是平滑速度
	public float smoothing = 5f;
    private float _pointY;
    // The initial offset from the target.
    //偏移量
    Vector3 offset;
    //相机抖动时间
    public float shake;
    //相机抖动幅度
    public float shakeAmount=0.1f;
	void Start() {
		// 计算目标位置和当前位置的偏移量
		offset = transform.position - target.position;
	}

	void FixedUpdate () {
        //如果相机抖动时间大于0，就要进行抖动
        if (shake > 0)
        {
            //抖动的幅度
            transform.position += Random.insideUnitSphere * shakeAmount;
            shake -= Time.deltaTime * 2;
        }
        else
        {
           
                Vector3 targetCamPos = target.position + offset;
                // 插值，平滑
                transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
                var dir = (target.position - transform.position).normalized;
                var rotation = Quaternion.LookRotation(dir);
                transform.rotation = rotation;

        }


    }
}