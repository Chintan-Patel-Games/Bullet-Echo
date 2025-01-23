using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Settings")]
    [SerializeField] private List<AudioItem> bgmItems;  // List of BGM AudioItems
    [SerializeField] private List<AudioItem> sfxItems;  // List of SFX AudioItems

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1.0f;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Play a specific BGM by its enum type
    public void PlayBGM(BGMList bgmType)
    {
        AudioItem bgmItem = bgmItems.Find(item => item.bgmType == bgmType);
        if (bgmItem == null)
        {
            Debug.LogWarning($"BGM with type '{bgmType}' not found.");
            return;
        }

        if (bgmItem.source.clip == bgmItem.clip) return; // Avoid restarting the same BGM

        StartCoroutine(SwitchBGM(bgmItem));
    }

    // Fade between BGM clips
    private IEnumerator SwitchBGM(AudioItem newBGMItem)
    {
        // Fade out the current BGM
        foreach (var item in bgmItems)
        {
            if (item.source.isPlaying)
            {
                float timer = 0;
                float initialVolume = item.source.volume;

                while (timer < fadeDuration)
                {
                    item.source.volume = Mathf.Lerp(initialVolume, 0, timer / fadeDuration);
                    timer += Time.deltaTime;
                    yield return null;
                }

                item.source.volume = 0;
                item.source.Stop();
            }
        }

        // Switch to the new clip and fade in
        newBGMItem.source.clip = newBGMItem.clip;
        newBGMItem.source.Play();

        float fadeInTimer = 0;
        while (fadeInTimer < fadeDuration)
        {
            newBGMItem.source.volume = Mathf.Lerp(0, 1.0f, fadeInTimer / fadeDuration);
            fadeInTimer += Time.deltaTime;
            yield return null;
        }

        newBGMItem.source.volume = 1.0f;
    }

    // Play a specific SFX by its enum type
    public void PlaySFX(SFXList sfxType)
    {
        AudioItem sfxItem = sfxItems.Find(item => item.sfxType == sfxType);
        if (sfxItem == null)
        {
            Debug.LogWarning($"SFX with type '{sfxType}' not found.");
            return;
        }

        sfxItem.source.PlayOneShot(sfxItem.clip);
    }
}