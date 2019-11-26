using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    //Set the sound array
    public Sound[] sounds;

    //Boolian for if the sound will be active
    public bool IsActive;

    //Set public Variables for editing the sound
    public float Volume;
    public float Frequency;
    public float Pitch;

    //Get Audio File from Audio Source component
    void Awake()
    {
       gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        AudioSource.volume = Volume;
    }

    //Set and get the volume slider value to the volume
    public void VolControl(float CurrentVol)
    {
        Volume = CurrentVol;
    }

    //Set and get the Frequency slider value to the Frequency
    public void FreqControl(float CurrentFreq)
    {
        Frequency = CurrentFreq;
    }

    //Set and get the Pitch slider value to the Pitch
    public void PitControl(float CurrentPit)
    {
        Pitch = CurrentPit;
    }

    //Reading the toggle to see if the sound is active
    public void ToggleSound(bool ActiveToggle)
    {
        IsActive = ActiveToggle;
    }
}
