using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // =====================================================
    // MUSIC MODE
    // =====================================================

    public enum MusicMode
    {
        Relaxing,
        Motivating,
        NoMusic
    }


    [Header("Music Settings")]
    public MusicMode musicMode = MusicMode.Relaxing;


    // =====================================================
    // MUSIC VOLUME
    // =====================================================

    [Header("Music Volume")]
    [Range(0f, 1f)]
    public float relaxingVolume = 0.8f;

    [Range(0f, 1f)]
    public float motivatingVolume = 0.1f;


    // =====================================================
    // AUDIO CLIPS
    // =====================================================

    [Header("Audio Clips")]
    public AudioClip relaxingMusic;
    public AudioClip motivatingMusic;


    // =====================================================
    // AUDIO SOURCE
    // =====================================================

    [Header("Audio Source")]
    public AudioSource audioSource;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (!InitializeAudioSource())
            return;

        ApplyMusicMode();
    }


    // =====================================================
    // INITIALIZE AUDIO SOURCE
    // =====================================================

    private bool InitializeAudioSource()
    {
        if (audioSource == null)
        {
            Debug.LogWarning(
                "MusicManager: AudioSource is not assigned!"
            );

            return false;
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        return true;
    }


    // =====================================================
    // APPLY MUSIC MODE
    // =====================================================

    public void ApplyMusicMode()
    {
        if (audioSource == null)
        {
            Debug.LogWarning(
                "MusicManager: AudioSource is not assigned!"
            );

            return;
        }


        switch (musicMode)
        {
            case MusicMode.Relaxing:
                PlayMusic(relaxingMusic, relaxingVolume);
                break;


            case MusicMode.Motivating:
                PlayMusic(motivatingMusic, motivatingVolume);
                break;


            case MusicMode.NoMusic:
                StopMusic();
                break;
        }


        Debug.Log(
            "Music mode applied: " + musicMode
        );
    }


    // =====================================================
    // PLAY MUSIC
    // =====================================================

    private void PlayMusic(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            Debug.LogWarning(
                "MusicManager: Music clip is not assigned!"
            );

            return;
        }


        // اگر همان آهنگ همین الان در حال پخش است،
        // فقط Volume را به‌روزرسانی کن.
        if (audioSource.isPlaying &&
            audioSource.clip == clip)
        {
            audioSource.volume = volume;
            return;
        }


        audioSource.Stop();

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = true;

        audioSource.Play();
    }


    // =====================================================
    // STOP MUSIC
    // =====================================================

    private void StopMusic()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
        audioSource.clip = null;
    }


    // =====================================================
    // PUBLIC STOP
    // =====================================================

    public void StopBackgroundMusic()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
        audioSource.clip = null;
    }
}