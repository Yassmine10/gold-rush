using UnityEngine;

/// <summary>
/// Gestionnaire audio pour Gold Rush.
/// Génère automatiquement des sons rétro-modernes s'il n'y a pas de fichier audio externe,
/// ou utilise des AudioClips assignés dans l'inspecteur.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Clips Audio Personnalisés (Optionnels)")]
    public AudioClip coinClip;
    public AudioClip jumpClip;
    public AudioClip victoryClip;
    public AudioClip hitClip;
    public AudioClip buttonClickClip;
    public AudioClip musicClip;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
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
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;

        // Générer les clips par défaut s'ils sont nuls
        if (coinClip == null) coinClip = CreateChimeClip(880f, 1320f, 0.15f);
        if (jumpClip == null) jumpClip = CreateJumpClip(0.18f);
        if (victoryClip == null) victoryClip = CreateFanfareClip(0.6f);
        if (buttonClickClip == null) buttonClickClip = CreatePopClip(0.08f);
        if (hitClip == null) hitClip = CreateHitClip(0.2f);
        if (musicClip == null) musicClip = CreateMusicClip();
        musicSource.clip = musicClip;

        SetSFXVolume(GameManager.GetSavedSetting("SFXVolume", 1f));
        SetMusicVolume(GameManager.GetSavedSetting("MusicVolume", 0.55f));
        if (musicClip != null && musicSource.volume > 0f) musicSource.Play();
    }

    public void PlayCoinSound() => PlaySFX(coinClip);
    public void PlayJumpSound() => PlaySFX(jumpClip);
    public void PlayVictorySound() => PlaySFX(victoryClip);
    public void PlayButtonClick() => PlaySFX(buttonClickClip);
    public void PlayHitSound() => PlaySFX(hitClip);

    public void PlayCustomSFX(AudioClip clip) => PlaySFX(clip);

    public void SetSFXVolume(float value)
    {
        if (sfxSource != null) sfxSource.volume = Mathf.Clamp01(value);
    }

    public void SetMusicVolume(float value)
    {
        if (musicSource == null) return;
        musicSource.volume = Mathf.Clamp01(value) * 0.28f;
        if (musicSource.volume > 0f && musicSource.clip != null && !musicSource.isPlaying) musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    private AudioClip CreateMusicClip()
    {
        const int sampleRate = 22050;
        const float beatLength = 0.5f;
        const int beats = 16;
        int sampleCount = Mathf.RoundToInt(sampleRate * beatLength * beats);
        float[] samples = new float[sampleCount];
        float[][] chords =
        {
            new[] { 261.63f, 329.63f, 392.00f }, // C
            new[] { 196.00f, 246.94f, 392.00f }, // G
            new[] { 220.00f, 261.63f, 329.63f }, // Am
            new[] { 174.61f, 220.00f, 261.63f }  // F
        };
        float[] bassNotes = { 130.81f, 98.00f, 110.00f, 87.31f };
        int samplesPerBeat = sampleCount / beats;

        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / sampleRate;
            int beat = i / samplesPerBeat;
            int chordIndex = (beat / 4) % chords.Length;
            float beatTime = (float)(i % samplesPerBeat) / sampleRate;
            float pad = 0f;
            for (int note = 0; note < chords[chordIndex].Length; note++)
                pad += Mathf.Sin(2f * Mathf.PI * chords[chordIndex][note] * time + note * 0.12f);
            pad *= 0.025f;

            float bassEnvelope = Mathf.Exp(-beatTime * 2.2f);
            float bass = Mathf.Sin(2f * Mathf.PI * bassNotes[chordIndex] * time) * bassEnvelope * 0.07f;
            int melodyIndex = (beat % 4 + chordIndex) % 3;
            float melodyFrequency = chords[chordIndex][melodyIndex] * 2f;
            float melodyEnvelope = Mathf.Exp(-beatTime * 5f);
            float melody = Mathf.Sin(2f * Mathf.PI * melodyFrequency * time) * melodyEnvelope * 0.045f;

            float edgeFade = Mathf.Min(1f, Mathf.Min(time / 0.08f, (sampleCount / (float)sampleRate - time) / 0.08f));
            samples[i] = Mathf.Clamp((pad + bass + melody) * Mathf.Clamp01(edgeFade), -0.25f, 0.25f);
        }

        AudioClip clip = AudioClip.Create("GoldRush_Adventure_Loop", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    // --- Génération procédurale de sons sans asset externe ---

    private AudioClip CreateChimeClip(float startFreq, float endFreq, float duration)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            float freq = Mathf.Lerp(startFreq, endFreq, t);
            float envelope = Mathf.Sin(t * Mathf.PI) * (1f - t);
            samples[i] = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate) * envelope * 0.4f;
        }

        AudioClip clip = AudioClip.Create("Coin_Procedural", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateJumpClip(float duration)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            float freq = Mathf.Lerp(220f, 480f, t);
            float envelope = 1f - t;
            samples[i] = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate) * envelope * 0.35f;
        }

        AudioClip clip = AudioClip.Create("Jump_Procedural", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreatePopClip(float duration)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            float freq = Mathf.Lerp(600f, 300f, t);
            float envelope = Mathf.Exp(-t * 15f);
            samples[i] = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate) * envelope * 0.3f;
        }

        AudioClip clip = AudioClip.Create("Click_Procedural", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateHitClip(float duration)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            float freq = Mathf.Lerp(150f, 50f, t);
            float noise = (Random.value * 2f - 1f) * 0.3f;
            float envelope = 1f - t;
            samples[i] = (Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate) + noise) * envelope * 0.4f;
        }

        AudioClip clip = AudioClip.Create("Hit_Procedural", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateFanfareClip(float duration)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        float[] notes = new float[] { 523.25f, 659.25f, 783.99f, 1046.50f }; // C5, E5, G5, C6
        int noteSamples = sampleCount / notes.Length;

        for (int i = 0; i < sampleCount; i++)
        {
            int noteIndex = Mathf.Clamp(i / noteSamples, 0, notes.Length - 1);
            float freq = notes[noteIndex];
            float noteT = (float)(i % noteSamples) / noteSamples;
            float envelope = Mathf.Sin(noteT * Mathf.PI) * (1f - noteT * 0.2f);
            samples[i] = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate) * envelope * 0.35f;
        }

        AudioClip clip = AudioClip.Create("Victory_Procedural", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
