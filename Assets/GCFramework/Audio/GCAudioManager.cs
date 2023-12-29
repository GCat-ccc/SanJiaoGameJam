using System;
using System.Collections;
using System.Collections.Generic;
using GCFramework.Extension;
using GCFramework.Resource;
using GCFramework.单例;
using JetBrains.Annotations;
using UnityEngine;

namespace GCFramework.Audio
{
    public class AudioClipInfo
    {
        public string AudioName;
        public AudioClip Clip;
    }
    
    public class AudioSettingData
    {
        /// <summary>
        /// 游戏音量
        /// </summary>
        public float GameVolume;
        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public float BgmVolume;
        /// <summary>
        /// 音效音量
        /// </summary>
        public float SoundVolume;
    }

    public enum AudioClipType
    {
        Bgm,
        Sfx
    }
    
    
    
    [RequireComponent(typeof(AudioSource))]
    public class GCAudioManager : SingletonMono<GCAudioManager>
    {
        private const string BgmPath = "Bgms/";
        private const string SfxPath = "Sfxs/";
        
        private AudioSource _bgmAudioSource = new AudioSource();
        private AudioSource _sfxAudioSource = new AudioSource();
        private readonly AudioSettingData _audioSettingData = new AudioSettingData();
        private readonly List<AudioClipInfo> _audioClipsCacheList = new List<AudioClipInfo>();

        public float GameVolume => _audioSettingData?.GameVolume ?? 1f;
        public float BgmVolume => _audioSettingData?.BgmVolume ?? 1f;
        public float SoundVolume => _audioSettingData?.SoundVolume ?? 1f;

        /// <summary>
        /// BGM过渡时间
        /// </summary>
        [SerializeField] private float fadeDuration = 1f;
        
        protected override void InitAwake()
        {
            base.InitAwake();
            var audioSources = GetComponents<AudioSource>();
            _bgmAudioSource = audioSources[0];
            _sfxAudioSource = audioSources[1];
        }

        private void Start()
        {
            // 从内存中读取音量数据
            _audioSettingData.GameVolume = 0.7f;
            _audioSettingData.BgmVolume = 0.5f;
            _audioSettingData.SoundVolume = 0.6f;
            _bgmAudioSource.volume = BgmVolume;
            _sfxAudioSource.volume = SoundVolume;
            
            _audioClipsCacheList.Clear();
            SetGameAudio(_audioSettingData.GameVolume, _audioSettingData.BgmVolume, _audioSettingData.SoundVolume);
        }

        /// <summary>
        /// 设置游戏各音量
        /// </summary>
        /// <param name="gameVolume"></param>
        /// <param name="bgmVolume"></param>
        /// <param name="soundVolume"></param>
        public void SetGameAudio(float gameVolume = 0.0f, float bgmVolume = 0.0f, float soundVolume = 0.0f)
        {
            _audioSettingData.GameVolume = gameVolume == 0.0f ? _audioSettingData.GameVolume : gameVolume;
            _audioSettingData.BgmVolume = bgmVolume == 0.0f ? _audioSettingData.BgmVolume : bgmVolume;
            _audioSettingData.SoundVolume = soundVolume == 0.0f ? _audioSettingData.SoundVolume : soundVolume;
        }

        #region BGM部分

        /// <summary>
        /// 播放BGM
        /// </summary>
        public void PlayBgm(string audioName)
        {
            if (TryGetBgmClip(audioName, out var clip))
            {
                if (_bgmAudioSource.isPlaying)
                {
                    StartCoroutine(SwitchBgmTransition(_bgmAudioSource, clip));
                    return;
                }
                
                _bgmAudioSource.clip = clip;
                _bgmAudioSource.loop = true;
                _bgmAudioSource.Play();
            }
        }
        
        /// <summary>
        /// 播放BGM
        /// </summary>
        public void PlayBgm(AudioSource audioSource, string audioName, float volume = 0.7f)
        {
            if (TryGetBgmClip(audioName, out var clip))
            {
                if (audioSource.isPlaying)
                {
                    StartCoroutine(SwitchBgmTransition(audioSource, clip));
                    return;
                }

                audioSource.volume = Math.Abs(volume - 0.7f) < 0.05f ? BgmVolume : volume;
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        /// <summary>
        /// 暂停Bgm
        /// </summary>
        public void PauseBgm()
        {
            if(!_bgmAudioSource.isPlaying) return;

            StartCoroutine(PauseBgmTransition());
        }

        /// <summary>
        /// 暂停bgm后渐出
        /// </summary>
        /// <returns></returns>
        private IEnumerator PauseBgmTransition()
        {
            yield return Transition(_bgmAudioSource, 0.0f);
            _bgmAudioSource.Stop();
        }

        /// <summary>
        /// 继续Bgm
        /// </summary>
        public void ResumeBgm()
        {
            if(_bgmAudioSource.isPlaying) return;

            StartCoroutine(ResumeBgmTransition());
        }

        /// <summary>
        /// 继续bgm后渐入
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResumeBgmTransition()
        {
            _bgmAudioSource.Play();
            yield return Transition(_bgmAudioSource, BgmVolume);
        }

        /// <summary>
        /// 切换BGM过渡
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        private IEnumerator SwitchBgmTransition(AudioSource audioSource, AudioClip clip)
        {
            yield return Transition(audioSource,0.0f);
            audioSource.Stop();

            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
            yield return Transition(audioSource, BgmVolume);
        }

        /// <summary>
        /// BGM音量过渡
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="targetVolume"></param>
        /// <returns></returns>
        private IEnumerator Transition(AudioSource audioSource, float targetVolume)
        {
            float timer = 0.0f;
            while (timer < fadeDuration)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
        }

        #endregion

        #region 音效部分

        /// <summary>
        /// 在指定AudioSource上播放音效
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="soundName"></param>
        /// <param name="volume"></param>
        /// <param name="pitch">播放速度</param>
        /// <param name="onPlayCompleted"></param>
        public void PlayOnceSound(AudioSource audioSource, string soundName,float volume = 0.7f, float pitch = 1.0f, [CanBeNull] Action onPlayCompleted = null)
        {
            if(audioSource == null) return;

            if (TryGetSfxClip(soundName, out var clip))
            {
                audioSource.pitch = pitch;
                audioSource.volume = Mathf.Abs(volume - 0.7f) < 0.05f ? SoundVolume : volume;
                audioSource.PlayOneShot(clip);
                audioSource.clip = clip;
                if (onPlayCompleted != null)
                    StartCoroutine(WhenPlayOver(audioSource, onPlayCompleted));
            }
        }

        /// <summary>
        /// 当播放完毕后执行
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="onPlayCompleted"></param>
        /// <returns></returns>
        private IEnumerator WhenPlayOver(AudioSource audioSource, Action onPlayCompleted)
        {
            if(!audioSource.isPlaying) yield break;

            float timer = 0.0f;
            while (audioSource && timer < audioSource.clip.length - 0.1f)
            {
                timer += Time.deltaTime;
                if(audioSource == null || !audioSource.gameObject.activeSelf)
                    StopCoroutine(nameof(WhenPlayOver));
                yield return null;
            }

            if (audioSource)
            {
                audioSource.pitch = 1.0f;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"播放音频的过程中组件挂载的对象被销毁，audioSource: {audioSource}");
#endif
            }
            onPlayCompleted?.Invoke();
        }

        /// <summary>
        /// 播放循环音效
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="soundName"></param>
        /// <param name="volume"></param>
        public void PlayLoopSound(AudioSource audioSource, string soundName, float volume = 0.7f)
        {
            if(audioSource == null) return;

            if (TryGetSfxClip(soundName, out var clip))
            {
                audioSource.volume = Mathf.Abs(volume - 0.7f) < 0.05f ? SoundVolume : volume;
                audioSource.loop = true;
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
        
        /// <summary>
        /// 播放音效，在全局，一般UI点击音效使用
        /// </summary>
        /// <param name="soundName"></param>
        /// <param name="onPlayCompleted"></param>
        public void PlayOnceSound(string soundName, Action onPlayCompleted = null)
        {
            if(_sfxAudioSource == null) return;

            if (TryGetSfxClip(soundName, out var clip))
            {
                _sfxAudioSource.volume = SoundVolume;
                _sfxAudioSource.PlayOneShot(clip);
                onPlayCompleted?.Invoke();
            }
        }

        #endregion

        #region 加载音频

        /// <summary>
        /// 从内存中加载音频资源
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="audioClipType"></param>
        /// <returns></returns>
        private AudioClip LoadAudio(string audioName, AudioClipType audioClipType)
        {
            AudioClip clip = null;
            clip = audioClipType == AudioClipType.Bgm 
                ? GCResourceManager.Ins.LoadRes<AudioClip>(BgmPath + audioName, AssetType.Audio) 
                : GCResourceManager.Ins.LoadRes<AudioClip>(SfxPath + audioName, AssetType.Audio);

            if (!clip)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"加载音效失败！audioName: {audioName}");
#endif
                return null;
            }

            _audioClipsCacheList.TryAdd(new AudioClipInfo
            {
                AudioName = audioName,
                Clip = clip
            });
            return clip;
        }

        /// <summary>
        /// 尝试获取BGM音频
        /// </summary>
        /// <param name="bgmName"></param>
        /// <returns></returns>
        private bool TryGetBgmClip(string bgmName, out AudioClip clip)
        {
            var audioClipInfo = _audioClipsCacheList.Find(x => x.AudioName == bgmName);
            if (audioClipInfo == null)
            {
                clip = LoadAudio(bgmName, AudioClipType.Bgm);
                return clip != null;
            }

            clip = audioClipInfo.Clip;
            return clip != null;
        }

        /// <summary>
        /// 尝试获取Sfx音频
        /// </summary>
        /// <param name="sfxName"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        private bool TryGetSfxClip(string sfxName, out AudioClip clip)
        {
            var audioClipInfo = _audioClipsCacheList.Find(x => x.AudioName == sfxName);
            if (audioClipInfo == null)
            {
                clip = LoadAudio(sfxName, AudioClipType.Sfx);
                return clip != null;
            }

            clip = audioClipInfo.Clip;
            return clip != null;
        }

        #endregion

        private void OnDisable()
        {
            _audioClipsCacheList.Clear();
            // 将音频音量数据存入硬盘
        }
    }
}