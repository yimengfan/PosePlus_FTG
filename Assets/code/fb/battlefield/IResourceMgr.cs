using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.BattleField
{
    public interface IExtPlayer
    {
        void PlayEffect(string name, Vector3 pos, int dir);

        void PlayEffect(string name, Transform follow, Vector3 pos, bool isfollow, int dir);
        int PlayEffectLooped(string name, Transform follow, Vector3 pos, bool isfollow, int dir);
        void CloseEffectLooped(int effid);

        void PlaySoundOnce(string name);
        void CleanAllEffect();
    }
   public interface IResourceMgr :IExtPlayer
    {
        GameObject CreateChar(string respath);
    }
}
