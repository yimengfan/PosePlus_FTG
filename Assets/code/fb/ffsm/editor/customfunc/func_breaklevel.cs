using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;


    class func_breaklevel:ICutomFunc
    {
        public string funcclass
        {
            get { return "breaklevel"; }
        }
        public void Init()
        {

        }

        public void OnGUI(FB.FFSM.BlockFunc func)
        {
            EditorGUILayout.HelpBox("这个功能是配置func_breaklevel", MessageType.Info);
            func.intParam0 = EditorGUILayout.IntField("中断等级", func.intParam0);
        }
    }
