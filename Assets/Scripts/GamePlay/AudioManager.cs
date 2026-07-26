using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundEntry
{
    public SoundId id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

/// Central audio system. Self-bootstraps from a prefab in
/// Resources/AudioManager.prefab before any scene loads, so it never
/// needs to be manually placed in a scene. Configure every sound clip
/// once, on that prefab, and call AudioManager.Instance.PlaySFX(...)
/// from anywhere.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] private List<SoundEntry> soundEntries = new List<SoundEntry>();

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.6f;

    private AudioSource sfxSource;
    private Dictionary<SoundId, SoundEntry> soundLookup;
    private readonly Dictionary<SoundId, AudioSource> loopingSources = new Dictionary<SoundId, AudioSource>();
    private Coroutine musicFadeRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        GameObject prefab = Resources.Load<GameObject>("AudioManager");

        if (prefab == null)
        {
            Debug.LogError("AudioManager: no prefab found at Resources/AudioManager. Create one to enable sound.");
            return;
        }

        Instantiate(prefab);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        soundLookup = new Dictionary<SoundId, SoundEntry>();
        foreach (var entry in soundEntries)
        {
            if (entry.clip != null)
                soundLookup[entry.id] = entry;
        }
    }

    /// Fires a one-shot sound. Safe to call repeatedly/overlapping —
    /// each call layers rather than cutting off the previous one.
    public void PlaySFX(SoundId id)
    {
        if (!soundLookup.TryGetValue(id, out SoundEntry entry))
        {
            Debug.LogWarning($"AudioManager: no clip assigned for {id}.");
            return;
        }

        sfxSource.PlayOneShot(entry.clip, entry.volume);
    }

    /// Starts or stops a continuous looping sound tied to a SoundId
    /// (e.g. footsteps). Idempotent — calling with the same state
    /// repeatedly does nothing extra.
    public void SetLoopingSound(SoundId id, bool play)
    {
        if (play)
        {
            if (loopingSources.ContainsKey(id))
                return;

            if (!soundLookup.TryGetValue(id, out SoundEntry entry))
            {
                Debug.LogWarning($"AudioManager: no clip assigned for {id}.");
                return;
            }

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = entry.clip;
            source.volume = entry.volume;
            source.loop = true;
            source.playOnAwake = false;
            source.Play();

            loopingSources[id] = source;
        }
        else
        {
            if (!loopingSources.TryGetValue(id, out AudioSource source))
                return;

            source.Stop();
            Destroy(source);
            loopingSources.Remove(id);
        }
    }

    /// Crossfades to a new music track. Calling with the clip already
    /// playing does nothing.
    public void PlayMusic(AudioClip clip, float fadeDuration = 1f)
    {
        if (musicSource == null || clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);

        musicFadeRoutine = StartCoroutine(CrossfadeMusic(clip, fadeDuration));
    }

    public void StopMusic(float fadeDuration = 1f)
    {
        if (musicSource == null)
            return;

        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);

        musicFadeRoutine = StartCoroutine(FadeOutMusic(fadeDuration));
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip, float duration)
    {
        float startVolume = musicSource.volume;
        float t = 0f;

        while (t < duration && musicSource.isPlaying)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / duration);
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = musicSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        musicSource.Stop();
    }
}