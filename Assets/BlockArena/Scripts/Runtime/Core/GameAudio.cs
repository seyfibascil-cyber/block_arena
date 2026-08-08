using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameAudio : MonoBehaviour
{
    private const string MusicKey = "BlockArena.Settings.Music";
    private const string SoundKey = "BlockArena.Settings.Sound";
    private const int SampleRate = 22050;

    private static GameAudio instance;
    private AudioSource musicSource;
    private AudioSource effectSource;
    private AudioSource resultSource;
    private AudioClip menuMusicClip;
    private AudioClip gameMusicClip;
    private AudioClip moveClip;
    private AudioClip obstacleClip;
    private AudioClip winClip;
    private AudioClip loseClip;
    private AudioClip buttonClip;
    private Coroutine resultStopRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (instance != null) return;
        GameObject host = new GameObject("GameAudio");
        DontDestroyOnLoad(host);
        instance = host.AddComponent<GameAudio>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        musicSource = gameObject.AddComponent<AudioSource>();
        effectSource = gameObject.AddComponent<AudioSource>();
        resultSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = 0.30f;
        effectSource.volume = 0.65f;
        resultSource.volume = 0.78f;

        menuMusicClip = Resources.Load<AudioClip>("BlockArena/Audio/MenuMusic");
        gameMusicClip = Resources.Load<AudioClip>("BlockArena/Audio/GameMusic");
        musicSource.clip = GetMusicForScene(SceneManager.GetActiveScene());
        moveClip = CreateSweep("Move", 0.12f, 330f, 520f, 0.18f);
        obstacleClip = Resources.Load<AudioClip>("BlockArena/Audio/Obstacle") ??
            CreateSweep("Obstacle", 0.18f, 150f, 75f, 0.30f);
        buttonClip = CreateSweep("Button", 0.07f, 520f, 680f, 0.12f);
        winClip = Resources.Load<AudioClip>("BlockArena/Audio/VictoryCrowd") ??
            CreateJingle("Win", new[] { 523.25f, 659.25f, 783.99f, 1046.5f });
        loseClip = Resources.Load<AudioClip>("BlockArena/Audio/DefeatCrowd") ??
            CreateJingle("Lose", new[] { 392f, 329.63f, 261.63f, 196f });
        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplySettings();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip nextMusic = GetMusicForScene(scene);
        if (musicSource.clip == nextMusic) return;
        musicSource.Stop();
        musicSource.clip = nextMusic;
        ApplySettings();
    }

    private AudioClip GetMusicForScene(Scene scene)
    {
        return scene.name == "Game" ? gameMusicClip : menuMusicClip;
    }

    private void Update()
    {
        bool shouldPlay = PlayerPrefs.GetInt(MusicKey, 1) == 1;
        if (shouldPlay && !musicSource.isPlaying) musicSource.Play();
        else if (!shouldPlay && musicSource.isPlaying) musicSource.Pause();
    }

    public static void ApplySettings()
    {
        if (instance == null) return;
        bool enabled = PlayerPrefs.GetInt(MusicKey, 1) == 1;
        if (enabled && !instance.musicSource.isPlaying) instance.musicSource.Play();
        else if (!enabled && instance.musicSource.isPlaying) instance.musicSource.Pause();
    }

    public static void PlayMove() => Play(instance?.moveClip);
    public static void PlayObstacle() => Play(instance?.obstacleClip);
    public static void PlayWin()
    {
        if (instance != null) instance.PlayResult(instance.winClip, 3f);
    }

    public static void PlayLose()
    {
        if (instance != null) instance.PlayResult(instance.loseClip, 0f);
    }
    public static void PlayButton() => Play(instance?.buttonClip);

    private static void Play(AudioClip clip)
    {
        if (instance == null || clip == null ||
            PlayerPrefs.GetInt(SoundKey, 1) != 1) return;
        instance.effectSource.PlayOneShot(clip);
    }

    private void PlayResult(AudioClip clip, float maximumDuration)
    {
        if (clip == null || PlayerPrefs.GetInt(SoundKey, 1) != 1) return;
        resultSource.Stop();
        if (resultStopRoutine != null)
        {
            StopCoroutine(resultStopRoutine);
            resultStopRoutine = null;
        }
        resultSource.volume = 0.78f;
        resultSource.clip = clip;
        resultSource.Play();
        if (maximumDuration > 0f)
        {
            resultStopRoutine = StartCoroutine(
                FadeAndStopResult(maximumDuration, 0.7f)
            );
        }
    }

    private IEnumerator FadeAndStopResult(float totalSeconds, float fadeSeconds)
    {
        float fullVolumeSeconds = Mathf.Max(0f, totalSeconds - fadeSeconds);
        yield return new WaitForSecondsRealtime(fullVolumeSeconds);

        float elapsed = 0f;
        while (elapsed < fadeSeconds && resultSource.isPlaying)
        {
            elapsed += Time.unscaledDeltaTime;
            resultSource.volume = Mathf.Lerp(
                0.78f,
                0f,
                Mathf.Clamp01(elapsed / fadeSeconds)
            );
            yield return null;
        }

        resultSource.Stop();
        resultSource.volume = 0.78f;
        resultStopRoutine = null;
    }

    private static AudioClip CreateSweep(string name, float seconds, float from,
        float to, float volume)
    {
        int count = Mathf.CeilToInt(seconds * SampleRate);
        float[] data = new float[count];
        float phase = 0f;
        for (int i = 0; i < count; i++)
        {
            float t = i / (float)count;
            phase += Mathf.Lerp(from, to, t) * 2f * Mathf.PI / SampleRate;
            float envelope = Mathf.Sin(Mathf.PI * t);
            data[i] = Mathf.Sin(phase) * envelope * volume;
        }
        AudioClip clip = AudioClip.Create(name, count, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip CreateJingle(string name, float[] notes)
    {
        const float noteLength = 0.16f;
        int perNote = Mathf.RoundToInt(noteLength * SampleRate);
        float[] data = new float[perNote * notes.Length];
        for (int n = 0; n < notes.Length; n++)
        {
            for (int i = 0; i < perNote; i++)
            {
                float t = i / (float)perNote;
                float envelope = Mathf.Sin(Mathf.PI * t);
                data[n * perNote + i] = Mathf.Sin(2f * Mathf.PI * notes[n] * i / SampleRate)
                    * envelope * 0.24f;
            }
        }
        AudioClip clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip CreateMusic()
    {
        float[] melody = { 261.63f, 329.63f, 392f, 329.63f, 293.66f, 349.23f, 440f, 349.23f };
        const float beat = 0.5f;
        int beatSamples = Mathf.RoundToInt(beat * SampleRate);
        float[] data = new float[beatSamples * melody.Length * 2];
        for (int i = 0; i < data.Length; i++)
        {
            int note = (i / beatSamples) % melody.Length;
            float local = (i % beatSamples) / (float)beatSamples;
            float fade = Mathf.Min(1f, local * 8f) * Mathf.Min(1f, (1f - local) * 6f);
            float time = i / (float)SampleRate;
            float lead = Mathf.Sin(2f * Mathf.PI * melody[note] * time) * 0.10f;
            float bass = Mathf.Sin(2f * Mathf.PI * melody[note] * 0.5f * time) * 0.07f;
            data[i] = (lead + bass) * fade;
        }
        AudioClip clip = AudioClip.Create("Block Arena Theme", data.Length, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
