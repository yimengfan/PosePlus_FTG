using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Proxies;
using UnityEngine.UI;

/// <summary>
/// 命令列表
/// </summary>
public class CommandList : MonoBehaviour
{
    List<UnityEngine.UI.Image> tags = new List<Image>();
    public List<Sprite> sprites = new List<Sprite>();
    int showcount = 12;
    void Start()
    {
        for (int i = 0; i < showcount; i++)
        {
            GameObject o = new GameObject();
            tags.Add(o.AddComponent<Image>());
            o.transform.localPosition = new Vector3(-238 + 40 * i, 0, 0);
            o.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
            o.transform.SetParent(this.transform, false);
            tags[i].enabled = false;
        }
    }

    int GetSpriteIndex(Vector2 dir)
    {
        int x = 0;
        if (dir.x < -0.5) x = -1;
        if (dir.x > 0.5) x = 1;
        int y = 0;
        if (dir.y < -0.5) y = -1;
        if (dir.y > 0.5) y = 1;

        if (x == 0 && y == 0)
            return -1;
        int i = (y + 1) * 3 + (x + 1);
        if (i > 3) i--;
        return i;

    }
    int GetSpriteIndex(PadButton btn)
    {
        if (btn == PadButton.Func)
            return 8;
        if (btn == PadButton.Func_Down)
            return 9;
        if (btn == PadButton.Func_Up)
            return 10;
        if (btn == PadButton.Func_Forward)
            return 11;
        return -1;
    }
    // Update is called once per frame

    void Update()
    {
        List<int> displayList = new List<int>();
        //Debug.LogWarning(joy.dirState.x + " :" + Time.realtimeSinceStartup);
        var list = FBJoy2.g_joy.GetCommandList();

        Command? last = null;
        foreach (var l in list)
        {
            if (last == null)
            {
                int index = GetSpriteIndex(l.dir);
                if (index > -1)
                {
                    displayList.Add(index);
                }
                index = GetSpriteIndex(l.state);
                if (index > -1)
                {
                    displayList.Add(index);
                }
            }
            else
            {
                if (last.Value.dir != l.dir)
                {
                    int index = GetSpriteIndex(l.dir);
                    if (index > -1)
                    {
                        displayList.Add(index);
                    }
                }
                if (last.Value.state != l.state)
                {
                    int index = GetSpriteIndex(l.state);
                    if (index > -1)
                    {
                        displayList.Add(index);
                    }
                }
            }
            last = l;
        }

        //关闭用不到的
        for (int i = displayList.Count; i < showcount; i++)
        {
            tags[i].enabled = false;
        }
        int ibegin = displayList.Count - showcount;
        if (ibegin < 0) ibegin = 0;
        for (int i = 0; i < Mathf.Min(showcount, displayList.Count); i++)
        {
            tags[i].enabled = true;
            tags[i].sprite = sprites[displayList[ibegin + i]];
        }
    }


}
