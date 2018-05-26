using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;
namespace EDCN
{
    class stateTree
    {
        public string name;
        public bool isOpen;
        public List<stateTree> childList;
    }
}

public class Window_PickState :EditorWindow 
{
    static EDCN.stateTree tomST;
    static List<FB.FFSM.TreeStateItem> statel;
    static Action<string> _callback;
    float space = 0;
    Vector2 pos;
    public static void Show(FB.FFSM.TreeStateItem[] stateList, Action<string> callback,bool isAndMe )
    {
        var window = EditorWindow.GetWindow<Window_PickState>(true, "Window_StateLeadState", true);
        statel = new List<FB.FFSM.TreeStateItem> ();
        tomST = new EDCN.stateTree ();
        foreach (FB.FFSM.TreeStateItem tom in stateList) 
        {
            statel.Add(tom);
        }
        tomST = spanningTree("",tomST);
        if (isAndMe)
        {
            foreach (var item in Window_StateTable.stateTable.allStates)
            {
                if (!isRepeat(tomST, item.name, false))
                {
                    EDCN.stateTree a = new EDCN.stateTree();
                    a.name = item.name;
                    a.childList = new List<EDCN.stateTree>();
                    tomST.childList.Add(a);
                }

            }
        }
        _callback = callback;
    }

    void OnGUI()
    {
        if (UnityEngine.GUILayout.Button("any"))
        {
            this.Close();
            _callback("any");
        }
        pos = UnityEngine.GUILayout.BeginScrollView(pos);
        space=15;
        layreNumber = 0;
        showTree (tomST);
        if (UnityEngine.GUILayout.Button("==Close=="))
        {
            this.Close();
            _callback(null);
        }
        GUILayout.EndScrollView();
    }

    int layreNumber=0;
    void showTree(EDCN.stateTree tree1)
    {

        if (tree1.name !="")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(space*layreNumber);
            if (tree1.childList.Count != 0) {
                tree1.isOpen = GUILayout.Toggle(tree1.isOpen, "", GUILayout.Width(10));
            }
            bool shiFouYiYou = false;
            foreach (var item in Window_StateTable.stateTable.allStates)
            {
                if(tree1.name==item.name)
                {
                    shiFouYiYou = true;
                }
            }
            if (!shiFouYiYou) 
            {
                Color color1 = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label("miss", GUILayout.Width(30));
                GUI.color = color1;
            }
            if (GUILayout.Button(tree1.name, GUILayout.Width(tree1.name.Length * 8 + 8), GUILayout.Height(20))) 
            {
                this.Close();
                _callback(tree1.name);
            }
            GUILayout.EndHorizontal();
        }
        if (tree1.isOpen)
        {
            layreNumber++;
            for (int i = 0; i < tree1.childList.Count; i++)
            {
                showTree(tree1.childList[i]);
            }
            layreNumber--;
        }

    }
    static EDCN.stateTree spanningTree (string empty,EDCN.stateTree tom)
    {
        EDCN.stateTree tomS1=new EDCN.stateTree();
        tom = new EDCN.stateTree();
        tom.name = empty;
        tom.isOpen = false;
        tom.childList = new List<EDCN.stateTree> ();
        for (int i=0; i<statel.Count; i++) 
        {
            if (statel[i].parent == tom.name)
            {
                tom.isOpen = true;
                string a = statel[i].name;
                tom.childList.Add(spanningTree(a, tomS1));
            }
        }
        return tom;
    }
    static bool isRepeat(EDCN.stateTree node1,string str,bool isGo)
    {
        isGo=(str == node1.name);
        if(!isGo)
        {
            foreach (EDCN.stateTree node2 in node1.childList) 
            {
                isGo=isRepeat(node2, str, isGo);
                if (isGo) 
                {
                    return isGo;
                }

            }
        }
        return isGo;
    }

}
