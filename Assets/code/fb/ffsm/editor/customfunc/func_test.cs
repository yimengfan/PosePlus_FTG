using System;
using System.Collections.Generic;

using System.Reflection.Emit;
using System.Text;
using UnityEngine;

public class func_test : ICutomFunc
{

    public string funcclass
    {
        get { return "test"; }
    }
    public void Init()
    {

    }
    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        GUILayout.Label("Test custom UI");
    }
}
