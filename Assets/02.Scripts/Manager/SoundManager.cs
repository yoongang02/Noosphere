using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
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

    public void PlaySound(string path)
    {
        AudioClip clip = LoadAudioClip(GetSoundFullPath(path));
        if (clip == null)
            return;
        _soundSource.PlayOneShot(clip);
    }
    public void ClearLoadedAudioClip() => _loadedClip.Clear();
}