using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoSingleton<AudioController>
{
    [ShowInInspector, ReadOnly] private AudioSource musicSource;
    [ShowInInspector, ReadOnly] private AudioSource sfxSource;
    [Space]
    [ShowInInspector, ReadOnly] private Dictionary<SoundType, AudioClip> sounds = new Dictionary<SoundType, AudioClip>();
    [ShowInInspector, ReadOnly] private Dictionary<MusicType, AudioClip> musics = new Dictionary<MusicType, AudioClip>();

    protected override void InitializeBehaviour()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;

        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        LoadAudioClips();
    }

    private void LoadAudioClips()
    {
        sounds = new Dictionary<SoundType, AudioClip>();
        musics = new Dictionary<MusicType, AudioClip>();

        AudioClip[] soundClips = Resources.LoadAll<AudioClip>("Sounds");
        AudioClip[] musicClips = Resources.LoadAll<AudioClip>("Musics");

        foreach (AudioClip sound in soundClips)
        {
            SoundType soundType = (SoundType)soundClips.ToList().IndexOf(sound);
            sounds.Add(soundType, sound);
        }

        foreach (AudioClip music in musicClips)
        {
            MusicType musicType = (MusicType)musicClips.ToList().IndexOf(music);
            musics.Add(musicType, music);
        }
    }

    public void PlaySfx(SoundType sound) => PlaySfx(sound, 1f);
    public void PlaySfx(SoundType sound, float volume)
    {
        AudioClip sfxClip = sounds.GetValueOrDefault(sound);

        sfxSource.volume = volume;
        sfxSource.PlayOneShot(sfxClip);
    }

    public void PlayMusic() => PlayMusic((MusicType)Random.Range(0, 6));
    public void PlayMusic(MusicType music) => PlayMusic(music, 1f);
    public void PlayMusic(MusicType music, float volume) => PlayMusic(music, volume, 1f);
    public void PlayMusic(MusicType music, float volume, float transition)
    {
        AudioClip musicClip = musics.GetValueOrDefault(music);

        StartCoroutine(TransitionEffect(musicClip, volume, transition));
    }

    public void ResumeMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }
    public void StopMusic()
    {
        StartCoroutine(FadeToStop(musicSource.volume));
    }

    private IEnumerator TransitionEffect(AudioClip clip, float volume, float transition)
    {
        if (musicSource.clip == clip) yield break;

        float lastVolume = musicSource.volume;

        yield return FadeToStop(lastVolume);

        musicSource.clip = clip;

        yield return FadeToPlay(clip, volume, transition);
    }
    private IEnumerator FadeToStop(float lastVolume)
    {
        if (musicSource.isPlaying)
        {
            float t = 0;

            WaitForEndOfFrame delay = new WaitForEndOfFrame();

            for (t = 0; t < 1; t += Time.deltaTime)
            {
                musicSource.volume = (1 - (t / 1)) * lastVolume;
                yield return delay;
            }
        }

        musicSource.Stop();
    }
    private IEnumerator FadeToPlay(AudioClip clip, float volume, float transition)
    {
        float t = 0;

        musicSource.Play();
        musicSource.volume = 0;

        WaitForEndOfFrame delay = new WaitForEndOfFrame();

        for (t = 0; t < transition; t += Time.deltaTime)
        {
            musicSource.volume = (t / transition) * volume;
            yield return delay;
        }

        musicSource.volume = volume;
    }
}

public enum SoundType
{
    Clique,
    Derrota,
    Pop,
    Vitoria,
}

public enum MusicType
{
    Musica_1,
    Musica_2,
    Musica_3,
    Musica_4,
    Musica_5,
    Musica_6,
}