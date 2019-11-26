using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

    public float Volume;
    public float Frequency;
    public float Pitch;

    void Awake()
    {
        gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        
    }
    public void VolControl(float CurrentVol)
    {
        Volume = CurrentVol;
    }
    public void FreqControl(float CurrentFreq)
    {
        Frequency = CurrentFreq;
    }
    public void PitControl(float CurrentPit)
    {
        Pitch = CurrentPit;
    }
}
