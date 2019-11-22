using System.Collections;
using System.Collections.Generic;
//using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class AudioTinker : MonoBehaviour {

    private AudioSource audioSource;
	private AudioClip audioClip;
	private float freq = 50;
    private const int octave = 12;
    private string lastChord = "";
    
    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioClip = CreateToneAudioClip(freq);
		PlayOutAudio();
	}
    

    // Public APIs
    public void PlayOutAudio() {
        audioSource.PlayOneShot(audioClip);
    }


    public void StopAudio() {
        audioSource.Stop();
    }

	void Update()
	{
		if (!audioSource.isPlaying)
		{
			/*freq += Random.Range(1, 200);
			audioClip = CreateToneAudioClip(freq);
			audioSource.PlayOneShot(audioClip);*/
			playChord(ChordGen());
		}
	}
    
    
    // Private 
    private AudioClip CreateToneAudioClip(float frequency) {
        int sampleDurationSecs = 1;
        int sampleRate = 44100;
        int sampleLength = sampleRate * sampleDurationSecs;
        float maxValue = 1f / 4f;
        
        var audioClip = AudioClip.Create("tone", sampleLength, 1, sampleRate, false);
        
        float[] samples = new float[sampleLength];
        for (var i = 0; i < sampleLength; i++) {
            float s = Mathf.Sin(2.0f * Mathf.PI * frequency * ((float) i / (float) sampleRate));
            float v = s * maxValue;
            samples[i] = v;
        }

        audioClip.SetData(samples, 0);
        return audioClip;
    }

	private float Notes(float noteNum)
	{
		float frequency = 440 * Mathf.Pow(2.0f, (noteNum/12.0f));
		return (frequency);
	}

	private void playChord(AudioClip[] chord)
	{
		for(int i = 0; i < chord.Length; i++)
		{
			audioSource.PlayOneShot(chord[i]);
		}
	}

	private AudioClip[] ChordGen()
	{
		string[] chords = new string[4] { "C", "G7", "Am", "Dm6" };
        string chordToMake = "";
        while(chordToMake == lastChord)
        {
            chordToMake = chords[Random.Range(0, chords.Length)];
        }
        lastChord = chordToMake;
        AudioClip[] chord = new AudioClip[0];
		if (chordToMake == "C")
		{
			chord = new AudioClip[3];
			chord[0] = CreateToneAudioClip(Notes(-33+octave));
			chord[1] = CreateToneAudioClip(Notes(-29 + octave));
			chord[2] = CreateToneAudioClip(Notes(-26 + octave));
		}
		else if(chordToMake == "G7")
		{
			chord = new AudioClip[4];
			chord[0] = CreateToneAudioClip(Notes(-26 + octave));
			chord[1] = CreateToneAudioClip(Notes(-34 + octave));
			chord[2] = CreateToneAudioClip(Notes(-19 + octave));
			chord[3] = CreateToneAudioClip(Notes(-16 + octave));
		}
		else if (chordToMake == "Am")
		{
			chord = new AudioClip[3];
			chord[0] = CreateToneAudioClip(Notes(-36 + octave));
			chord[1] = CreateToneAudioClip(Notes(-21 + octave));
			chord[2] = CreateToneAudioClip(Notes(-17 + octave));
		}
		else if (chordToMake == "Dm6")
		{
			chord = new AudioClip[4];
			chord[0] = CreateToneAudioClip(Notes(-31 + octave));
			chord[1] = CreateToneAudioClip(Notes(-28 + octave));
			chord[2] = CreateToneAudioClip(Notes(-36 + octave));
			chord[3] = CreateToneAudioClip(Notes(-34 + octave));
		}
		return chord;
	}

    
#if UNITY_EDITOR
    //[Button("Save Wav file")]
    private void SaveWavFile() {
        string path = EditorUtility.SaveFilePanel("Where do you want the wav file to go?", "", "", "wav");
        var audioClip = CreateToneAudioClip(1500);
        SaveWavUtil.Save(path, audioClip);
    }
#endif
}