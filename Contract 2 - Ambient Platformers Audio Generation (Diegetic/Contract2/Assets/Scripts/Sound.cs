using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound{

    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 1f)]
    public float pitch;
    [Range(0f, 60f)]
    public float delay;

    public bool loop;
    public bool mute;

    [HideInInspector]
    public AudioSource source;
}
