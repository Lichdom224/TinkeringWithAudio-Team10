using System.Collections;
using System.Collections.Generic;
//using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class AudioTinker : MonoBehaviour {

	// Initialising variables
    private AudioSource audioSource;
	private AudioClip audioClip;

	private float freq = 50;
	private string lastChord = "";

	// Octave can be changed in inspector or changed using Up and Down arrows
	// Octave affects the frequency that each note is generated as
	// E.g A4 = Octave value of 3
	public int octave = 2;
    

    // Start is called before the first frame update
    void Start() {
		// GetComponent<AudioSource>() will return the audioSource attatched to the gameObject
		// audioSource will be used to Play out sounds in the scene
        audioSource = GetComponent<AudioSource>();

		// audioClip is created using default frequency to begin with
        audioClip = CreateToneAudioClip(freq);

		//PlayOutAudio();
	}


	// If PlayOutAudio is called, the currently saved audioClip will be played once
	public void PlayOutAudio() {
        audioSource.PlayOneShot(audioClip);
    }


	// If the audioSource needs to be stopped, calling StopAudio() will stop what is currently playing
    public void StopAudio() {
        audioSource.Stop();
    }


	// Update is called once per frame
	void Update()
	{
		// If there is nothing playing on the audio source, generate and play a random chord
		if (!audioSource.isPlaying)
		{
			PlayChord(ChordGen());

			#region TestCode - Random Increasing Frequency
			/*freq += Random.Range(1, 200);
			audioClip = CreateToneAudioClip(freq);
			audioSource.PlayOneShot(audioClip);*/
			#endregion
		}

		// Checking inputs for up and down arrows
		// If UpArrow is pressed, increase octave by 1
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			octave++;
		}
		// If DownArrow is pressed, decrease octave by 1
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			octave--;
		}
	}
    

	// CreateToneAudioClip() is used to create a sound wave using a passed frequency
    private AudioClip CreateToneAudioClip(float frequency) {
		// sampleDurationSecs determines how long the wave will play for, value 2 is chosen so that the chord changes don't drown out the melody
        int sampleDurationSecs = 2;
        int sampleRate = 44100; // 44100 is default for music
        int sampleLength = sampleRate * sampleDurationSecs;
        float maxValue = 1f / 4f;
        
        var audioClip = AudioClip.Create("tone", sampleLength, 1, sampleRate, false);
        
		// Every sample is assigned data which is assigned to the local audioClip and then returned at the end of the function
        float[] samples = new float[sampleLength];
        for (var i = 0; i < sampleLength; i++) {
            float s = Mathf.Sin(2.0f * Mathf.PI * frequency * ((float) i / (float) sampleRate));
            float v = s * maxValue;
            samples[i] = v;
        }

		// Assign all of the generated samples to local audioClip, then return
        audioClip.SetData(samples, 0);
        return audioClip;
    }


	// Notes() is used to create and return the frequency for a passed note number
	private float Notes(float noteNum)
	{
		float frequency = 440 * Mathf.Pow(2.0f, (noteNum/12.0f));
		return (frequency);
	}


	// playChord() is called after generating a chord and is used to play every note from the chord
	private void PlayChord(AudioClip[] chord)
	{
		// Iterates through the length of the function and plays each note simultaneously
		for (int i = 0; i < chord.Length; i++)
		{
			audioSource.PlayOneShot(chord[i]);
		}
	}


	// ChordGen() picks a random chord from a pre-defined array and returns an array of frequencies as a result
	private AudioClip[] ChordGen()
	{
		//string[] chords = new string[4] { "C", "G7", "Am", "Dm6" };
		string[] chords = new string[3] { "Cm", "Fm", "Gm"};

		// Picking a random chord from the array to create note frequencies and return
		string chordToMake = chords[Random.Range(0, chords.Length)];
		// The while loop is used to ensure the same chord is not played twice (or more) in a row
		while (chordToMake == lastChord)
        {
            chordToMake = chords[Random.Range(0, chords.Length)];
        }

		// Records the chord that was just picked for next time the function runs
        lastChord = chordToMake;

		// chord is assigned a blank array here, to avoid errors, in case no statements are true
        AudioClip[] chord = new AudioClip[0];

		// Piano-C Major is the code in use here, Basic Chords is currently commented out
		#region Basic Chords
		/*if (chordToMake == "C")
		{
			chord = new AudioClip[3];

			chord[0] = CreateToneAudioClip(Notes(-33 + (octave*12)));
			chord[1] = CreateToneAudioClip(Notes(-29 + (octave*12)));
			chord[2] = CreateToneAudioClip(Notes(-26 + (octave*12)));
		}
		else if(chordToMake == "G7")
		{
			chord = new AudioClip[4];

			chord[0] = CreateToneAudioClip(Notes(-26 + (octave*12)));
			chord[1] = CreateToneAudioClip(Notes(-34 + (octave*12)));
			chord[2] = CreateToneAudioClip(Notes(-19 + (octave*12)));
			chord[3] = CreateToneAudioClip(Notes(-16 + (octave*12)));
		}
		else if (chordToMake == "Am")
		{
			chord = new AudioClip[3];

			chord[0] = CreateToneAudioClip(Notes(-36 + (octave*12)));
			chord[1] = CreateToneAudioClip(Notes(-21 + (octave*12)));
			chord[2] = CreateToneAudioClip(Notes(-17 + (octave*12)));
		}
		else if (chordToMake == "Dm6")
		{
			chord = new AudioClip[4];

			chord[0] = CreateToneAudioClip(Notes(-31 + (octave*12)));
			chord[1] = CreateToneAudioClip(Notes(-28 + (octave*12)));
			chord[2] = CreateToneAudioClip(Notes(-36 + (octave*12)));
			chord[3] = CreateToneAudioClip(Notes(-34 + (octave*12)));
		}*/
		#endregion
		#region Piano-C Major
		// Statements check which chord was randomly chosen and chord is assigned each AudioClip before being returned
		if (chordToMake == "Cm")
		{
			chord = new AudioClip[3];

			// Notes() here takes the note number and returns a frequency for use in CreateToneAudioClip()
			// '+ (octave * 12)' is used to determine the current octave that the notes are being generated in
			chord[0] = CreateToneAudioClip(Notes(-33 + (octave * 12)));
			chord[1] = CreateToneAudioClip(Notes(-29 + (octave * 12)));
			chord[2] = CreateToneAudioClip(Notes(-26 + (octave * 12)));
		}
		else if(chordToMake == "Fm")
		{
			chord = new AudioClip[3];

			chord[0] = CreateToneAudioClip(Notes(-28 + (octave * 12)));
			chord[1] = CreateToneAudioClip(Notes(-36 + (octave * 12)));
			chord[2] = CreateToneAudioClip(Notes(-33 + (octave * 12)));
		}
		else if (chordToMake == "Gm")
		{
			chord = new AudioClip[3];

			chord[0] = CreateToneAudioClip(Notes(-26 + (octave * 12)));
			chord[1] = CreateToneAudioClip(Notes(-34 + (octave * 12)));
			chord[2] = CreateToneAudioClip(Notes(-31 + (octave * 12)));
		}
		#endregion

		// Return the final chord array
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