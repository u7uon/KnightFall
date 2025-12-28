using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Audio/SoundDatabase")]
public class SoundDatabase : ScriptableObject
{
    public List<SoundEntry> sfxs = new List<SoundEntry>();
}

[System.Serializable]
public class SoundEntry
{
    public string key;
    public AudioClip clip;

}