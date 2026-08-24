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


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogWarning(
                "MusicManager: AudioSource is not assigned!"
            );

            return;
        }


        audioSource.loop = true;

        audioSource.playOnAwake = false;


        ApplyMusicMode();
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


        Debug.Log(
            "Music mode applied: " +
            musicMode
        );
    }


    // =====================================================
    // PLAY MUSIC
    // =====================================================

    private void PlayMusic(
        AudioClip clip
    )
    {
        if (clip == null)
        {
            Debug.LogWarning(
                "MusicManager: Music clip is not assigned!"
            );

            return;
        }


        // اگر همان آهنگ در حال پخش است
        if (
            audioSource.isPlaying &&
            audioSource.clip == clip
        )
        {
            return;
        }


        audioSource.Stop();

        audioSource.clip = clip;

        audioSource.loop = true;

        audioSource.Play();
    }


    // =====================================================
    // STOP
    // =====================================================

    private void StopMusic()
    {
        audioSource.Stop();

        audioSource.clip = null;
    }


    // =====================================================
    // PUBLIC STOP
    // =====================================================

    public void StopBackgroundMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();

            audioSource.clip = null;
        }
    }
}