using UnityEngine;
using System.Collections.Generic;
using Game;
/// <summary>
/// 音频管理器
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    /// <summary>
    /// 背景音乐
    /// </summary>
    public AudioSource LoopAudioSource
    {
        private set;
        get;
    }
    /// <summary>
    /// 英雄移动音效
    /// </summary>
    public AudioSource HeroMoveAudio
    {
        get;
        private set;
    }
    /// <summary>
    /// 新手向导声音
    /// </summary>
    public AudioSource GuideVoice
    {
        get;
        private set;
    }
    /// <summary>
    /// 特效声音队列
    /// </summary>
    public Queue<AudioSource> EffectAudioSourceQueue
    {
        get;
        private set;
    }
    /// <summary>
    /// 长声音
    /// </summary>
    public Queue<AudioSource> LongVoiceAudioSourceQueue
    {
        get;
        private set;
    }
    /// <summary>
    /// 各英雄台词播放
    /// </summary>
    public Dictionary<long, AudioSource> HeroLinesVoiceDic
    {
        get;
        private set;
    }
    /// <summary>
    /// 游戏击杀音频列表
    /// </summary>
    public List<AudioClip> GameKillClipList
    {
        get;
        set;
    }
    /// <summary>
    /// 游戏击杀
    /// </summary>
    public AudioSource GameKillAudioSource
    {
        get;
        private set;
    }
    /// <summary>
    /// 英雄补刀获得金币声音
    /// </summary>
    public AudioSource HeroGetMoneySource
    {
        get;
        private set;
    }
    public AudioManager()
    {
        HeroLinesVoiceDic = new Dictionary<long, AudioSource>();
        GameKillClipList = new List<AudioClip>();
        EffectAudioSourceQueue = new Queue<AudioSource>();
        #region 背景音乐
        LoopAudioSource = LOLGameDriver.Instance.gameObject.AddComponent<AudioSource>();
        LoopAudioSource.loop = true;
        LoopAudioSource.playOnAwake = false;
        LoopAudioSource.volume = SystemConfig.LocalSetting.MusicVolume;
        #endregion
    }
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="adClip"></param>
    public void PlayBgAudio(AudioClip adClip)
    {
        if (LoopAudioSource == null || adClip == null || (LoopAudioSource.clip != null && LoopAudioSource.clip.name == adClip.name))
        {
            return;
        }
        LoopAudioSource.clip = adClip;
        LoopAudioSource.Play();
    }
    /// <summary>
    /// 停止播放背景音乐
    /// </summary>
    public void StopBgAudio()
    {
        if (LoopAudioSource == null)
        {
            return;
        }
        LoopAudioSource.Stop();
    }
    /// <summary>
    /// 播放声音特效
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public AudioSource PlayEffectAudio(AudioClip clip)
    {
        if (null == clip)
        {
            return null;
        }
        AudioSource ads = EffectAudioSourceQueue.Dequeue();
        ads.clip = clip;
        ads.volume = 1;
        ads.Play();
        EffectAudioSourceQueue.Enqueue(ads);
        return ads;
    }
    public AudioSource PlayLongVoiceAudio(AudioClip adClip)
    {
        if (null == adClip)
        {
            return null;
        }
        AudioSource ads = LongVoiceAudioSourceQueue.Dequeue();
        ads.clip = adClip;
      //  ads.volume = 1;
        ads.Play();
        EffectAudioSourceQueue.Enqueue(ads);
        return ads;
    }
    /// <summary>
    /// 更改音频音量
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    public void ChangeAudioVolume(AudioSource source, float value)
    {
        if (null == source || null == source.clip)
        {
            return;
        }
        source.volume = value;
    }
}
