using UnityEngine;

[System.Serializable]
public class AudioItem
{
    public AudioSource source;  // The AudioSource to play the clip
    public AudioClip clip;      // The AudioClip to be played
    public BGMList bgmType = BGMList.None;  // Default to None
    public SFXList sfxType = SFXList.None;  // Default to None
}