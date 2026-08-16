using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public enum MusicMode
    {
        Calm,
        Rhythmic,
        NoMusic
    }

    [Header("Music Settings")]
    public MusicMode musicMode = MusicMode.Calm;

    [Header("Audio Clips")]
    public AudioClip calmMusic;
    public AudioClip rhythmicMusic;

    [Header("Audio Source")]
    public AudioSource audioSource;

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("MusicManager: AudioSource is not assigned!");
            return;
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        switch (musicMode)
        {
            case MusicMode.Calm:
                PlayMusic(calmMusic);
                break;

            case MusicMode.Rhythmic:
                PlayMusic(rhythmicMusic);
                break;

            case MusicMode.NoMusic:
                StopMusic();
                break;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("MusicManager: Music clip is not assigned!");
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }

    private void StopMusic()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }
}