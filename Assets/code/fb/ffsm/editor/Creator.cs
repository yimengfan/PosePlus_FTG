using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;

class Creator
{
    [MenuItem("Assets/Create/FB/TreeStateConfig")]
    static void CreateTreeStateConfig()
    {

        string outpath = GetCreatePath();
        //Debug.Log("setpath=" + path);
        FB.FFSM.TreeStateConfig config = ScriptableObject.CreateInstance<FB.FFSM.TreeStateConfig>();



        AssetDatabase.CreateAsset(config, outpath);

    }
    [MenuItem("Assets/Create/FB/StateTable")]
    static void CreateStateTable()
    {

        string outpath = GetCreatePath();
        //Debug.Log("setpath=" + path);
        FB.FFSM.StateTable config = ScriptableObject.CreateInstance<FB.FFSM.StateTable>();



        AssetDatabase.CreateAsset(config, outpath);

    }



    [MenuItem("Assets/Create/FB/AI_stateTable")]
    static void CreateAIStateConfig()
    {
         var obj = ScriptableObject.CreateInstance<FB.PosePlus.AniClip>();
        string outpath = GetCreatePath();
        FB.FFSM.AI_StateTable config = ScriptableObject.CreateInstance<FB.FFSM.AI_StateTable>();
        AssetDatabase.CreateAsset(config, outpath);

    }

    [MenuItem("Assets/Create/FB/FlyItem")]
    static void CreateFlyItem()
    {
        var obj = ScriptableObject.CreateInstance<FB.PosePlus.AniClip>();
        var f = new FB.PosePlus.Frame();
        f.box_key = true;
        f.dot_key = true;
        f.key = true;
        obj.frames = new List<FB.PosePlus.Frame>();
        obj.frames.Add(f);
        string outpath = GetCreatePath();
        AssetDatabase.CreateAsset(obj, outpath);

    }
    static string GetCreatePath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (Selection.activeObject is UnityEditor.DefaultAsset)
        {

        }
        else
        {
            path = System.IO.Path.GetDirectoryName(path);
        }
        path = AssetDatabase.GenerateUniqueAssetPath(path + "/config");

        string app = System.IO.Path.GetDirectoryName(Application.dataPath);
        string fullpath = app + "/" + path;
        string realfull = fullpath + ".asset";
        string outpath = path + ".asset";
        for (int i = 0; ; i++)
        {

            if (i > 0)
            {
                realfull = fullpath + " " + i.ToString("D02") + ".asset";
                outpath = path + " " + i.ToString("D02") + ".asset";
            }
            if (System.IO.File.Exists(realfull))
                continue;
            else
                break;
        }

        return outpath;
    }
}

