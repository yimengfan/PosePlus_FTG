using UnityEngine;
using System.Collections;


public class CameraRect : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }
    Vector2 srcsize;
    public UnityEngine.UI.CanvasScaler[] scalerfix;
    public Vector2 minScreenSize = new Vector2(16, 9);
    public Vector2 maxScreenSize = new Vector2(3, 2);
    int lastScreenWidth = 0;
    int lastScreenHeight = 0;
    // Update is called once per frame
    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            FixCameraRect();
        }
    }
    public float finalasp=0;
    void FixCameraRect()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        float a1 = minScreenSize.x / minScreenSize.y;
        float a2 = maxScreenSize.x / maxScreenSize.y;
        float minasp = Mathf.Min(a1, a2);
        float maxasp = Mathf.Max(a1, a2);
        float asp = (float)lastScreenWidth / (float)lastScreenHeight;
        Rect rect = this.GetComponent<Camera>().rect;
        if (asp < minasp)
        {
            float newheight = lastScreenWidth / minasp;
            float oneheight = newheight / lastScreenHeight;
            rect = new Rect(0, (1 - oneheight) / 2.0f, 1.0f, oneheight);
            this.GetComponent<Camera>().rect = rect;
            asp = minasp;
        }
        else if (asp > maxasp)
        {
            float newwidth = lastScreenHeight * maxasp;
            float onewidth = newwidth / lastScreenWidth;
            rect = new Rect((1 - onewidth) / 2.0f, 0, onewidth, 1.0f);
            this.GetComponent<Camera>().rect = rect;
            asp = maxasp;
        }
        else
        {//原来漏了一个恢复的部分
            rect = new Rect(0, 0, 1, 1);
            this.GetComponent<Camera>().rect = rect;
        }
        finalasp = asp;
        if (scalerfix != null)
        {
            foreach (var s in scalerfix)
            {
                if (srcsize.y <= 0)
                    srcsize = s.referenceResolution;
                //Debug.Log("reset scale" + srcsize);

                float h = rect.height;
                Vector2 n = srcsize;
                n.y /= h;
                //Debug.Log("reset n" + n);
                s.referenceResolution = n;
            }
        }
    }
}
