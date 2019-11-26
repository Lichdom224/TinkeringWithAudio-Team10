using UnityEngine.Audio;
using UnityEngine;
// Shows this class in the array
[System.Serializable]

//Creates the sound class
public class Sound{
    public string Name;
    public AudioClip Clip;
    public float Volume;
    public float Pitch;

    [HideInInspector]
    public AudioSource Souce;

}
