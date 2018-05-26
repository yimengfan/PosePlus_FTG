using UnityEngine;
using System.Collections;

public class AudioPlayer : MonoBehaviour
{
    public bool useGUITest = false;

    public void OnGUI()
    {
        if (useGUITest)
        {
            if (GUI.Button(new Rect(0, 0, 100, 50), "Play Once 01"))
            {
                PlaySoundOnce("audio/npcfly");
            }
            if (GUI.Button(new Rect(0, 100, 100, 50), "Play Once 02"))
            {
                PlaySoundOnce("audio/k");
            }

            if (GUI.Button(new Rect(100, 00, 100, 50), "Play back 01"))
            {
                PlayBack("audio/backnormal");
            }

            if (GUI.Button(new Rect(100, 100, 100, 50), "Play back 02"))
            {
                PlayBack("audio/backfight");
            }

            if (GUI.Button(new Rect(100, 200, 100, 50), "Stop back"))
            {
                StopBack();
            }


            if (GUI.Button(new Rect(200, 00, 100, 50), "PlayLoop 01"))
            {
                StartSound("audio/bossin", 0.5f);
            }

            if (GUI.Button(new Rect(200, 100, 100, 50), "StopLoop sound"))
            {
                StopSound("audio/bossin");
            }
            if (GUI.Button(new Rect(200, 200, 100, 50), "Stop all"))
            {
                StopAllSound(2.5f);
            }

        }
    }


    static AudioPlayer g_this = null;
    public static AudioPlayer Instance()
    {
        return g_this;
    }
    private AudioSource assGoabel = null;
    void Start()
    {
        if (assGoabel == null)
        {
            assGoabel = gameObject.AddComponent<AudioSource>();
        }
        g_this = this;
        //Debuger.Log("init audioplayer");
    }

    void Update()
    {
        foreach (var v in fadesound)
        {

            float vv = v.Key.volume;
            vv -= Time.deltaTime / v.Value;

            if (vv <= 0)
            {
                //立即停止
                StopSound(v.Key.clip.name, 0);
                fadesound.Remove(v.Key);
                return;
            }
            else
            {
                v.Key.volume = vv;
            }
        }
    }

    /// <summary>
    /// 只播放一次音乐
    /// </summary>
    /// <param name="name">音乐路径</param>
    /// <param name="volume">音量大小，0无，1最大</param>
    public void PlaySoundOnce(string name, float volume = 1.0f)
    {
        //if (ass == null)
        //{
        //    ass = gameObject.GetComponent<AudioSource>();
        //}
        //if (ScreenStateMgr.Instance()._GlobalData.ifMusicOn)
        {
            AudioClip c = Resources.Load<AudioClip>(name);
            assGoabel.PlayOneShot(c as AudioClip, volume);
        }
        
    }


    System.Collections.Generic.Dictionary<string, AudioSource> sounds = new System.Collections.Generic.Dictionary<string, AudioSource>();
    System.Collections.Generic.Queue<AudioSource> freesound = new System.Collections.Generic.Queue<AudioSource>();
    System.Collections.Generic.Dictionary<AudioSource, float> fadesound = new System.Collections.Generic.Dictionary<AudioSource, float>();
    AudioSource getFreeSource()
    {
        if (freesound.Count > 0)
            return freesound.Dequeue();
        else
            return this.gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// 连续播放音效，特殊音效
    /// </summary>
    /// <param name="name">音乐路径</param>
    /// <param name="volume">音量大小，0无，1最大</param>
    public void StartSound(string name, float volume = 1.0f)
    {
        if (sounds.ContainsKey(name))
        {
            var s = sounds[name];
            //确保不再fadesound中
            fadesound.Remove(s);

            s.volume = volume;
        }
        else
        {
            //return;
            AudioClip c = Resources.Load<AudioClip>(name);
            var s = getFreeSource();
            //确保不再fadesound中
            fadesound.Remove(s);

            s.clip = c;
            s.volume = volume;
            s.loop = true;
            s.time = 0;
            s.Play();
            sounds[name] = s;
        }
    }
    /// <summary>
    /// 停止播放连续音效
    /// </summary>
    /// <param name="name">音乐路径</param>
    /// <param name="fadetime">消失速度，0立刻停止，1延迟1秒消失</param>
    public void StopSound(string name, float fadetime = 1.0f)
    {
        if (sounds.ContainsKey(name) == false) return;
        AudioSource s = sounds[name];

        if (fadetime > 0.01f)
        {
            fadesound[s] = fadetime;
        }
        else
        {
            s.Stop();

            sounds.Remove(name);
            freesound.Enqueue(s);
        }
    }
    /// <summary>
    /// 停止播放所有音效
    /// </summary>
    /// <param name="fadetime">消失速度</param>
    public void StopAllSound(float fadetime = 1.0f)
    {
        System.Collections.Generic.List<string> keys = new System.Collections.Generic.List<string>(sounds.Keys);
        foreach (var n in keys)
        {
            StopSound(n, fadetime);
        }

    }


    string backsound = null;
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    /// <param name="volume"></param>
    public void PlayBack(string name, float volume = 0.5f)
    {
        //if (ScreenStateMgr.Instance()._GlobalData.ifMusicOn)
        {
            if (string.IsNullOrEmpty(backsound) == false)
            {
                StopSound(backsound);
            }
            StartSound(name, volume);
            backsound = name;
        }
    }
    /// <summary>
    /// 停止播放背景音乐
    /// </summary>
    public void StopBack()
    {
        if (string.IsNullOrEmpty(backsound) == false)
        {
            StopSound(backsound, 0);
        }
    }

}
