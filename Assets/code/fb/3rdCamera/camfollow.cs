using UnityEngine;
using System.Collections;

//必须挂在Camera上面
public class camfollow : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {//
        InitCamPos();
    }
    public void InitCamPos()
    {
        if (FollowObj == null)
            return;
        lookatNow = lookatWant = FollowObj.TransformPoint(lookatAdd);
        eyeNow = eyeWant = CalcEyeByLookatYPD(yaw, pitch, distance, lookatWant);
        this.transform.position = eyeNow;
        this.transform.LookAt(lookatNow, Vector3.up);
        this.GetComponent<Camera>().fieldOfView = fov;
    }
    public Transform FollowObj;
    public Vector3 lookatAdd =new  Vector3(0,1,1);

    public float yaw = 0;
    public float pitch = 68;
    public float fov = 30;
    public float distance = 10;

    public float lookatspeed = 5.0f;
    public float eyespeed = 4.0f;
    // Update is called once per frame
    Vector3 lookatWant;
    Vector3 lookatNow;
    Vector3 eyeWant;
    Vector3 eyeNow;
    void Update()
    {
        if (FollowObj == null)
            return;
        //计算目标
        lookatWant = FollowObj.TransformPoint(lookatAdd);
        eyeWant = CalcEyeByLookatYPD(yaw, pitch, distance, lookatWant);

        //插值
        lookatNow = Vector3.Lerp(lookatNow, lookatWant, lookatspeed * Time.deltaTime);
        eyeNow = Vector3.Lerp(eyeNow, eyeWant, eyespeed * Time.deltaTime);
        
        //生效
        this.transform.position = eyeNow;
        this.transform.LookAt(lookatNow, Vector3.up);

        //测试代码
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("keydown");
            this.FollowObj.Rotate(Vector3.up, 180);
        }
    }

    static Vector3 CalcEyeByLookatYPD(float yaw, float pitch, float distance, Vector3 lookat)
    {
        Vector3 forward = new Vector3(0, -distance, 0);
        forward = Quaternion.Euler(-pitch, yaw, 0) * forward;
        return lookat - forward;
    }
}
