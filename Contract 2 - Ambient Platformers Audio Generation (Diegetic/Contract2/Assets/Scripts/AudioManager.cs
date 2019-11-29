using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class AudioManager : MonoBehaviour{

    public Sound[] sounds;
    public float timedelay;
    public Sound s;

    void Awake(){
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.mute = s.mute;
            timedelay = s.delay;

        }
    }
    
    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        if (s != null)
            StartCoroutine(Wait(s));
            
    }
    void Start()
    {
        Play("Birds");
        
    }
    IEnumerator Wait(Sound s)
    {
        yield return new WaitForSeconds(timedelay);
        s.source.Play();
    }
}
