using System.Collections;
using System.Collections.Generic;
//using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/*
Contract 3 - Melody Generation (Non-Diegetic Audio)

Link to GitHub repository: https://github.com/ColeGGilbert/TinkeringWithAudio-Team10
Code and project authored by Cole Gilbert under the GPL 3.0 license
Script template provided in Comp120 Tinkering Audio lectures was used for basic functions

This script should be attatched to a gameObject with the audio source component.
The purpose of this script is to make a melody using elements of procedural generation to control the output of tones and chords through samples, output as audio clips.
CreateToneAudioClip() function was taken from the template for tinkering audio and modified for use inside of this project to fit the contract's needs.
The script uses public variables for sampleDurationSeconds and octave to allow for manipulation outside of the source code. Both of these variables can be changed using the inspector.
The script makes use of programming paradigms to create algorithms that are appropriate for use in the generation of a music track.
*/

[RequireComponent(typeof(AudioSource))]
public class AudioTinker : MonoBehaviour {

	// Initialising variables
    private AudioSource audioSource;
	private AudioClip audioClip;

	private float freq = 50;
	private string lastChord = "";
	private float melodyPlayDelay;
	private float melodyRestDelay;
	private float melodyRest;
	private int lastMelodyNote = 2;
	public int totalSamplesMade;
	private float chordDelay;
	private int currentChordSize;
	private AudioClip[] currentChordClips = new AudioClip[3];
	private AudioClip combinedSong;
	private List<AudioClip> generatedClips = new List<AudioClip>();
	private float songEndTimer = 5;

	// Octave can be changed in inspector or changed using Up and Down arrows
	// Octave affects the frequency that each note is generated as
	// E.g A4 = Octave value of 3
	public int octave = 2;

	// sampleDurationSeconds can be changed in inspector
	// sampleDurationSeconds affects how long each chord will last
	// Default is set to 2 so that chord changes are not overly common so not to drown out the melody
	public int sampleDurationSeconds = 2;

	// Start is called before the first frame update
	void Start() {
		// GetComponent<AudioSource>() will return the audioSource attatched to the gameObject
		// audioSource will be used to Play out sounds in the scene
        audioSource = GetComponent<AudioSource>();

		// audioClip is created using default frequency to begin with
        audioClip = CreateToneAudioClip(freq, sampleDurationSeconds, false);

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
		if (chordDelay <= 0)
		{
			PlayChord(ChordGen());
			chordDelay = 2f;

			#region TestCode - Random Increasing Frequency
			/*freq += Random.Range(1, 200);
			audioClip = CreateToneAudioClip(freq);
			audioSource.PlayOneShot(audioClip);*/
			#endregion
		}
		else
		{
			chordDelay -= Time.deltaTime;
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

		if(melodyRest <= 0.6f && melodyPlayDelay <= 0)
		{
			audioSource.PlayOneShot(MelodyGen());
			melodyPlayDelay = Random.Range(0.1f, 1f);
		}
		else if(melodyPlayDelay > 0)
		{
			melodyPlayDelay -= Time.deltaTime;
		}

		if(melodyRestDelay <= 0)
		{
			melodyRest = Random.Range(0f, 1f);
			melodyRestDelay = Random.Range(1f, 3f);
		}
		else
		{
			melodyRestDelay -= Time.deltaTime;
		}

		if(songEndTimer <= 0)
		{
			combinedSong = AudioClip.Create("MySong", totalSamplesMade, 1, 44100, false);
			float[] samples = new float[totalSamplesMade];
			int totalSamplesAdded = 0;
			
			for (int i = 0; i < generatedClips.Count; i++)
			{
				float[] samplesToAdd = new float[generatedClips[i].samples];
				generatedClips[i].GetData(samplesToAdd, 0);
				for(int j = 0; j < samplesToAdd.Length; j++)
				{
					samples[j + totalSamplesAdded] = samplesToAdd[j];
				}
				totalSamplesAdded += generatedClips[i].samples;
			}
			combinedSong.SetData(samples, 0);
			SaveWavFile();
			songEndTimer = 5;
		}
		else
		{
			songEndTimer -= Time.deltaTime;
		}
	}
    

	// CreateToneAudioClip() is used to create a sound wave using a passed frequency
    private AudioClip CreateToneAudioClip(float frequency, int seconds, bool halfSampleSize) {
		// sampleDurationSecs determines how long the wave will play for, value 2 is chosen as default so that the chord changes don't drown out the melody
        int sampleDurationSecs = seconds;
        int sampleRate = 44100; // 44100 is default for music
        int sampleLength = sampleRate * sampleDurationSecs;
        float maxValue = 1f / 4f;

		AudioClip audioClip;

		// Every sample is assigned data which is assigned to the local audioClip and then returned at the end of the function
		float[] samples;
		if (halfSampleSize)
		{
			samples = new float[sampleLength/2];
			audioClip = AudioClip.Create("tone", sampleLength / 2, 1, sampleRate, false);
			for (var i = 0; i < sampleLength/2; i++)
			{
				float s = Mathf.Sin(2.0f * Mathf.PI * frequency * ((float)i / (float)sampleRate));
				float v = s * maxValue;
				samples[i] = v;
			}
		}
		else
		{
			samples = new float[sampleLength];
			audioClip = AudioClip.Create("tone", sampleLength, 1, sampleRate, false);
			for (var i = 0; i < sampleLength; i++)
			{
				float s = Mathf.Sin(2.0f * Mathf.PI * frequency * ((float)i / (float)sampleRate));
				float v = s * maxValue;
				samples[i] = v;
			}
		}

		// Assign all of the generated samples to local audioClip, then return
		audioClip.SetData(samples, 0);
		if (halfSampleSize)
		{
			generatedClips.Add(audioClip);
		}
		else
		{
			currentChordClips[currentChordSize] = audioClip;
			currentChordSize++;
			
			if(currentChordSize == 3)
			{
				generatedClips.Add(CombineChordTones());
			}
		}
		totalSamplesMade += audioClip.samples;
        return audioClip;
    }

	// Notes() is used to create and return the frequency for a passed note number
	private float Notes(float noteNum)
	{
		float frequency = 440 * Mathf.Pow(2.0f, (noteNum/12.0f));
		return (frequency);
	}


	private AudioClip CombineChordTones()
	{
		int chordSampleSize = 0;
		for(int i = 0; i < 3; i++)
		{
			chordSampleSize += currentChordClips[i].samples;
		}
		AudioClip combinedChord = AudioClip.Create("chord", chordSampleSize, 1, 44100, false);

		float[] samples = new float[chordSampleSize];
		int totalSamplesAdded = 0;

		for (int i = 0; i < currentChordClips.Length; i++)
		{
			float[] samplesToAdd = new float[currentChordClips[i].samples];
			currentChordClips[i].GetData(samplesToAdd, 0);
			for (int j = 0; j < samplesToAdd.Length; j++)
			{
				samples[j + totalSamplesAdded] = samplesToAdd[j];
			}
			totalSamplesAdded += currentChordClips[i].samples;
		}

		currentChordSize = 0;

		combinedChord.SetData(samples, 0);
		return combinedChord;
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

			chord[0] = CreateToneAudioClip(Notes(-33 + (octave*12)), sampleDurationSeconds, false);
			chord[1] = CreateToneAudioClip(Notes(-29 + (octave*12)), sampleDurationSeconds, false);
			chord[2] = CreateToneAudioClip(Notes(-26 + (octave*12)), sampleDurationSeconds, false);
		}
		else if(chordToMake == "G7")
		{
			chord = new AudioClip[4];

			chord[0] = CreateToneAudioClip(Notes(-26 + (octave*12)), sampleDurationSeconds, false);
			chord[1] = CreateToneAudioClip(Notes(-34 + (octave*12)), sampleDurationSeconds, false);
			chord[2] = CreateToneAudioClip(Notes(-19 + (octave*12)), sampleDurationSeconds, false);
			chord[3] = CreateToneAudioClip(Notes(-16 + (octave*12)), sampleDurationSeconds, false);
		}
		else if (chordToMake == "Am")
		{
			chord = new AudioClip[3];

			chord[0] = CreateToneAudioClip(Notes(-36 + (octave*12)), sampleDurationSeconds, false);
			chord[1] = CreateToneAudioClip(Notes(-21 + (octave*12)), sampleDurationSeconds, false);
			chord[2] = CreateToneAudioClip(Notes(-17 + (octave*12)), sampleDurationSeconds, false);
		}
		else if (chordToMake == "Dm6")
		{
			chord = new AudioClip[4];

			chord[0] = CreateToneAudioClip(Notes(-31 + (octave*12)), sampleDurationSeconds, false);
			chord[1] = CreateToneAudioClip(Notes(-28 + (octave*12)), sampleDurationSeconds, false);
			chord[2] = CreateToneAudioClip(Notes(-36 + (octave*12)), sampleDurationSeconds, false);
			chord[3] = CreateToneAudioClip(Notes(-34 + (octave*12)), sampleDurationSeconds, false);
		}*/
		#endregion
		#region Piano-C Major
		// Statements check which chord was randomly chosen and chord is assigned each AudioClip before being returned
		if (chordToMake == "Cm")
		{
			chord = new AudioClip[3];

			// Notes() here takes the note number and returns a frequency for use in CreateToneAudioClip()
			// '+ (octave * 12)' is used to determine the current octave that the notes are being generated in
			chord[0] = CreateToneAudioClip(Notes(-33 + (octave * 12)), sampleDurationSeconds, false);
			chord[1] = CreateToneAudioClip(Notes(-29 + (octave * 12)), sampleDurationSeconds, false);
			chord[2] = CreateToneAudioClip(Notes(-26 + (octave * 12)), sampleDurationSeconds, false);
		}
		else if(chordToMake == "Fm")
		{
			chord = new AudioClip[3];

			chord[0] = CreateToneAudioClip(Notes(-28 + (octave * 12)), sampleDurationSeconds, false);
			chord[1] = CreateToneAudioClip(Notes(-36 + (octave * 12)), sampleDurationSeconds, false);
			chord[2] = CreateToneAudioClip(Notes(-33 + (octave * 12)), sampleDurationSeconds, false);
		}
		else if (chordToMake == "Gm")
		{
			chord = new AudioClip[3];

			chord[0] = CreateToneAudioClip(Notes(-26 + (octave * 12)), sampleDurationSeconds, false);
			chord[1] = CreateToneAudioClip(Notes(-34 + (octave * 12)), sampleDurationSeconds, false);
			chord[2] = CreateToneAudioClip(Notes(-31 + (octave * 12)), sampleDurationSeconds, false);
		}
		#endregion

		// Return the final chord array
		return chord;
	}

	private AudioClip MelodyGen()
	{
		/*int modifyNoteNumber = Random.Range(-2, 3);
		while (modifyNoteNumber == 0)
		{
			modifyNoteNumber = Random.Range(-2, 3);
		}
		lastMelodyNote = Mathf.Clamp(lastMelodyNote + modifyNoteNumber, -36, -21);
		return CreateToneAudioClip(Notes(lastMelodyNote + ((octave + 0.65f) * 12)), 1, true);*/
		int[] notesForMelody = new int[5] { -33, -31, -29, -26, -24 };
		int modifyNoteNumber = Random.Range(-2, 3);
		while (modifyNoteNumber == 0)
		{
			modifyNoteNumber = Random.Range(-2, 3);
		}
		lastMelodyNote = Mathf.Clamp(lastMelodyNote + modifyNoteNumber, 0, 4);
		return CreateToneAudioClip(Notes(notesForMelody[lastMelodyNote] + ((octave + 1) * 12)), 1, true);
	}

    
#if UNITY_EDITOR
    //[Button("Save Wav file")]
    private void SaveWavFile() {
        string path = EditorUtility.SaveFilePanel("Where do you want the wav file to go?", "", "", "wav");
        SaveWavUtil.Save(path, combinedSong);
    }
#endif
}