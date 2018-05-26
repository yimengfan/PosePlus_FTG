using UnityEngine;
using System.Collections;

public class clientmain : MonoBehaviour
{

    // Use this for initialization
    public Camera mainCamera;
    static CameraRect mrect = null;
    static Camera sCamera;
    //按键操作延时
    void Start()
    {
        //主相机赋值
        sCamera = mainCamera;
      

    }
    public static bool canInput = true;


    public static Camera getMainCamera() 
    {
        return sCamera;
    }
    
    void Update()
    {

    }
}
