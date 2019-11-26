using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound{
    public string Name;
    public AudioClip Clip;
    public float Volume;
    public float Pitch;

    [HideInInspector]
    public AudioSource Souce;

}
