using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(FB.FFSM.TreeStateConfig))]
public class TreeStateConfig_Inspector : Editor
{
    bool bShowTree = false;
    FB.FFSM.TreeStateConfig statetree;
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying||target==null)
        {
            base.OnInspectorGUI();
            return;
        }

        bShowTree = GUILayout.Toggle(bShowTree, "编辑树/修改值");
        if (bShowTree)
        {
            EditorGUILayout.HelpBox("这是一个AnimConfig树查看工具", MessageType.Info);
            statetree = target as FB.FFSM.TreeStateConfig;
            TreeNode t = BuildTree(statetree);
            if(t!=null)
                FillTree(t);
        }
        else
        {
            base.OnInspectorGUI();
        }
    }

    TreeNode BuildTree(FB.FFSM.TreeStateConfig config)
    {
        TreeNode tree = null;
        Dictionary<string, TreeNode> nodes = new Dictionary<string, TreeNode>();
        if(config.states != null)
        foreach(var a in config.states)
        {
            if(!nodes.ContainsKey(a.name))
            {
                nodes.Add(a.name, new TreeNode() { aniname = a.name });
            }
           
        }
        foreach(var a in config.states)
        {
            if(string.IsNullOrEmpty(a.parent))
            {
                tree = nodes[a.name];
            }
            else
            {
              if(nodes.ContainsKey(a.parent))
                nodes[a.parent].Add(nodes[a.name]);
            }
        }
        return tree;
    }
    void FillTree(TreeNode tree)
    {
       var oc = GUI.color;
       SetColor(tree.aniname);
       GUILayout.Label(tree.aniname);

       GUI.color = oc;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(24);
        EditorGUILayout.BeginVertical();
        foreach(var st in tree)
        {
            FillTree(st);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    void  SetColor(string name)
    {
        FB.FFSM.stateColor sc = FB.FFSM.stateColor.Normal;
       foreach(var t in statetree.states)
       {
           if(t.name ==  name)
           {
               sc =t.color;
               break;
           }
       }

        switch (sc)
        {
            case FB.FFSM.stateColor.Normal:
                break;
            case FB.FFSM.stateColor.Red:
                GUI.color = Color.red;
                break;
            case FB.FFSM.stateColor.Yellow:
                GUI.color = Color.yellow;
                break;
            case FB.FFSM.stateColor.Blue:
                GUI.color = Color.blue;
                break;
            default:
                break;
        }

    }
    class TreeNode:List<TreeNode>
    {
        public string aniname;
    }
    //从一个Animator中获取所有的Animation

}

