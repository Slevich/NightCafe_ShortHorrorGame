using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    #region Fields
    [Header("Audio source to play clips."), SerializeField] private AudioSource _sourceToPlay;
    [Header("Clips to play."), SerializeField] private AudioClip[] _clips;
    #endregion

    #region Methods
    public void PlaySoundByIndex(int ClipIndex)
    {
        if (_clips == null || _clips.Length == 0)
            return;

        if (ClipIndex < 0 || ClipIndex >= _clips.Length)
            return;

        AudioClip clip = _clips[ClipIndex];
        PlaySound(clip);
    }

    public void PlayRandomSound()
    {
        if (_clips == null || _clips.Length == 0)
            return;

        int clipIndex = Random.Range(0, _clips.Length - 1);
        AudioClip clip = _clips[clipIndex];
        PlaySound(clip);
    }

    public void PlaySound(AudioClip clip)
    {
        if (_sourceToPlay == null || clip == null)
            return;

        _sourceToPlay.PlayOneShot(clip);
    }
    #endregion
}
