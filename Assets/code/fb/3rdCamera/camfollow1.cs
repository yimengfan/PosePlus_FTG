using UnityEngine;
using System.Collections;

//必须挂在Camera上面
public class Camfollow
{

    // Use this for initialization
    public void Start()
    {
        InitCamPos();
    }
    public void InitCamPos()
    {
        cam = clientmain.getMainCamera().gameObject;

        eyeNow = eyeWant = CalcEyeByLookatYPD(yaw, pitch, distance, lookatWant);
        cam.transform.position = eyeNow;
        cam.transform.LookAt(lookatNow, Vector3.up);
        cam.GetComponent<Camera>().fieldOfView = fov;
        if (FollowObj == null)
            return;
        lookatNow = lookatWant = FollowObj.TransformPoint(lookatAdd);
    }
    public GameObject cam;
    public Transform FollowObj;
    public Vector3 lookatAdd = new Vector3(0, 1.5f, 1);

    public float yaw = 0;
    public float pitch = 68;
    public float fov = 30;
    public float distance = 6.6f;

    public float lookatspeed = 5.0f;
    public float eyespeed = 4.0f;
    // Update is called once per frame
    Vector3 lookatWant;
    Vector3 lookatNow;
    Vector3 eyeWant;
    Vector3 eyeNow;
    /// <summary>
    /// 1、camfollow1脚本
    /// 2、摄像机插值跟踪
    /// </summary>
    public void Update()
    {
        if (FollowObj == null)
            return;
        //计算目标
        lookatWant = FollowObj.TransformPoint(lookatAdd);
        //Debug.LogError("lookatAdd =" + lookatAdd);
        eyeWant = CalcEyeByLookatYPD(yaw, pitch, distance, lookatWant);
        if (FocusTimer > 0)
        {
            FocusTimer -= Time.deltaTime;
            lookatWant = focus.TransformPoint(new Vector3(0, lookatAdd.y, 0));
            eyeWant = CalcEyeByLookatYPD(yaw, pitch, distance * 0.5f, lookatWant);

        }
        //插值
        //Debug.LogError("lookatWant=" + lookatWant);
        lookatNow = Vector3.Lerp(lookatNow, lookatWant, lookatspeed * Time.deltaTime);
        eyeNow = Vector3.Lerp(eyeNow, eyeWant, eyespeed * Time.deltaTime);
        if (shockTimer > 0)
        {
            shockTimer -= Time.deltaTime;
            if (shockTimer < 0)
                shockTimer = 0;
            float dist = 0.20f;
            float disteye = 0.40f;
            lookatNow += new Vector3((Random.value - 0.5f) * shockTimer * dist, (Random.value - 0.5f) * shockTimer * dist, 0);
            eyeNow += new Vector3((Random.value - 0.5f) * shockTimer * disteye, (Random.value - 0.5f) * shockTimer * disteye, 0);
        }
        //生效
        cam.transform.position = eyeNow;
        cam.transform.LookAt(lookatNow, Vector3.up);

        //测试代码
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("keydown");
            FollowObj.Rotate(Vector3.up, 180);
        }
    }
    float shockTimer = 0;
    public void Shock()
    {
        shockTimer = 1.0f;
    }
    Transform focus;
    float FocusTimer = 0;
    public void Focus(Transform focus,float timer)
    {
        this.focus = focus;
        this.FocusTimer = timer;
    }
    /// <summary>
    /// 1、摄像机旋转跟随
    /// 2、第四个参数是目标点位置
    /// </summary>
    /// <param name="yaw"></param>
    /// <param name="pitch"></param>
    /// <param name="distance"></param>
    /// <param name="lookat">这个是目标点</param>
    /// <returns></returns>
    static Vector3 CalcEyeByLookatYPD(float yaw, float pitch, float distance, Vector3 lookat)
    {
        Vector3 forward = new Vector3(0, -distance, 0);
        forward = Quaternion.Euler(-pitch, yaw, 0) * forward;
        return lookat - forward;
    }
}
