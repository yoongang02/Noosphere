using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource _bgmSource = null;
    [SerializeField]
    private AudioSource _soundSource = null;
    private Dictionary<string, AudioClip> _loadedClip = new Dictionary<string, AudioClip>();
    private AudioClip LoadAudioClip(string fullPath)
    {
        AudioClip clip = null;
        if (_loadedClip.TryGetValue(fullPath, out clip))
            return clip;

        clip = Resources.Load<AudioClip>(fullPath);
        if (clip == null)
        {
            Debug.LogError($"[SoundManager.LoadAudioClip.InvalidPath]{fullPath}");
            return null;
        }

        _loadedClip.Add(fullPath, clip);
        return clip;
    }

    private static string GetBGMFullPath(string path) => Define._bgmRoot + "/" + path;

    public void LoadBGM(string path) => LoadAudioClip(GetBGMFullPath(path));

    public void PlayBGM(string path)
    {
        AudioClip clip = LoadAudioClip(GetBGMFullPath(path));
        if (clip == null)
            return;
        
        _bgmSource.clip = clip;
        _bgmSource.Play();
    }

    public void StopBGM() => _bgmSource.Stop();

    private static string GetSoundFullPath(string path) => Define._soundRoot + "/" + path;

    public void LoadSound(string path) => LoadAudioClip(GetSoundFullPath(path));

    // public void PlaySound(string path)
    // {
    //     AudioClip clip = LoadAudioClip(GetSoundFullPath(path));
    //     if (clip == null)
    //         return;
    //     _soundSource.PlayOneShot(clip);
    // }

    public void PlaySound(string path,int loopCount)
    {
        AudioClip clip = LoadAudioClip(GetSoundFullPath(path));
        if (clip == null)
            return;
   
        StartCoroutine(PlaySoundRepeatedly(clip, loopCount));
    }
    private IEnumerator PlaySoundRepeatedly(AudioClip clip, int repeatCount)
    {
        for(int i = 0; i < repeatCount; i++)
        {
            _soundSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }

        EffectManager.Instance.isEffectEnd = true;
    }

    public void ClearLoadedAudioClip() => _loadedClip.Clear();
}