using System;
using System.Collections.Generic;
using UnityEngine;
namespace FB.FFSM
{

    public class TreeStateConfig : ScriptableObject
    {
        [SerializeField]
        public TreeStateItem[] states;

        public TreeNode GetNode(string name)
        {
            if(mapNode==null)
            {
                mapNode = new Dictionary<string, TreeNode>();
                foreach(var s in states)
                {
                    mapNode[s.name] = new TreeNode();
                    mapNode[s.name].name = s.name;
                }
                foreach(var s in states)
                {
                    if(mapNode.ContainsKey( s.parent))
                    {
                        if (mapNode[s.parent].children == null)
                            mapNode[s.parent].children = new List<TreeNode>();
                        mapNode[s.parent].children.Add(mapNode[s.name]);
                        mapNode[s.name].parent = mapNode[s.parent];
                    }
                }
            }
            TreeNode node = null;
            if (mapNode.TryGetValue(name, out node))
            {
                return node;
            }
            else
            {
                return null;
            }
        }
       
        private Dictionary<string, TreeNode> mapNode;
    }
    public class TreeNode
    {
        public TreeNode parent;
        public string  name;
        public List<TreeNode> children;

    }
    public enum stateColor
    {
        Normal,
        Red,
        Yellow,
        Blue,
    }
    [Serializable]
    public class TreeStateItem
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public string parent;

        [SerializeField]
        public stateColor color = stateColor.Normal;
    }

    [Serializable]
    public class StringPair
    {
        [SerializeField]
        public string key;

        [SerializeField]
        public string value;
    }
}