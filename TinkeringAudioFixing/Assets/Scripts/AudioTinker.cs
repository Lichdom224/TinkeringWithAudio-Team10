using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Contract 3 - Melody Generation (Non-Diegetic Audio)
///
///	Link to GitHub repository: https://github.com/ColeGGilbert/TinkeringWithAudio-Team10
///	Code and project authored by Cole Gilbert under the GPL 3.0 license
///	Pair programming undertaken with Nicholas Lane, with assistance in logic of how to make good sounding melody with James Gill
///	Script template provided in Comp120 Tinkering Audio lectures was used for basic functions e.g. SaveWavUtil
///
///	This script should be attatched to a gameObject with the audio source component.
///	The purpose of this script is to make a melody using elements of procedural generation to control the output of tones and chords through samples, output as audio clips.
///	CreateToneAudioClip() function was taken from the template for tinkering audio and modified for use inside of this project to fit the contract's needs.
///	The script uses public variables for chordDurationSeconds and octave to allow for manipulation outside of the source code.Both of these variables can be changed using the inspector.
///	The script makes use of programming paradigms to create algorithms that are appropriate for use in the generation of a music track.
/// </summary>

// RequireComponent ensures that the AudioSource is attatched to the object, if not, it forces the AudioSource component onto the gameObject
[RequireComponent(typeof(AudioSource))]
public class AudioTinker : MonoBehaviour {

	// Initialising variables
    private AudioSource audioSource;

	private string lastChord = "";

	// Timer for time between chords being played
	private float chordDelay;

	// Initialise melody variables that determine how often melody notes are played, some of these variables can be changed in inspector
	// melodyPlayDelay determines the time between each note being played
	[Range(0.1f, 1.0f)]
	public float melodyPlayDelayMin = 0.25f;
	[Range(1.0f, 5.0f)]
	public float melodyPlayDelayMax = 1.0f;
	private float melodyPlayDelay;

	// melodyRestDelay determines the time between each new chance at causing a melody rest
	[Range(0.5f, 3.0f)]
	public float melodyRestDelayMin = 1.0f;
	[Range(3.0f, 10.0f)]
	public float melodyRestDelayMax = 3.0f;
	private float melodyRestDelay;

	// melodyRestChance determines the chance of the melody taking a break
	[Range(0.0f, 1f)]
	public float melodyRestChance = 0.4f;
	private float melodyRest;

	// Saves the last melody note to stop duplication of notes
	private int lastMelodyNote = 2;

	// Initialising variables that determine the complete song to be saved after generation
	public int totalSamplesMade;
	private AudioClip[] currentChordClips = new AudioClip[3];
	private AudioClip combinedSong;
	private List<AudioClip> generatedClips = new List<AudioClip>();

	// Song duration determines how long each clip of music should cover before being asked to save
	// [Range...] is used to make the songDuration variable a slider in the inspector that also clamps the value between a min and max value
	[Range(2.0f, 120.0f)]
	public float songDuration = 12;
	private float songEndTimer;

	// Octave can be changed in inspector or changed using Up and Down arrows
	// Octave affects the frequency that each note is generated as
	// E.g A4 = Octave value of 3
	public int octave = 2;

	// chordDurationSeconds can be changed in inspector
	// chordDurationSeconds affects how long each chord will last
	// Default is set to 2 so that chord changes are not overly common so not to drown out the melody
	public int chordDurationSeconds = 2;

	// Initialise booleans for whether to generate chords and melody or not
	public bool toggleChords = true;
	public bool toggleMelody = true;

	// Start is called before the first frame update
	void Start() {
		// GetComponent<AudioSource>() will return the audioSource attatched to the gameObject
		// audioSource will be used to Play out sounds in the scene
        audioSource = GetComponent<AudioSource>();

		// The current song timer is set to the length of the song set by the user (or it is defaulted)
		songEndTimer = songDuration;
	}


	// If PlayOutAudio is called, the currently saved audioClip will be played once
	public void PlayOutAudio(AudioClip audioClip) {
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
		if (chordDelay <= 0 && toggleChords)
		{
			PlayChord(ChordGen());
			chordDelay = chordDurationSeconds;
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


		// Melody timing is processed here and is used to determine whether the next note should be played or not
		// melodyRest is used to create a break from melody to stop from over use of melody notes
		// melodyPlayDelay is used to create a small gap between tones, giving them a chance to finish playing before the next note plays
		if(melodyRest >= melodyRestChance && melodyPlayDelay <= 0 && toggleMelody)
		{
			// Generates and plays the next note
			PlayOutAudio(MelodyGen());

			melodyPlayDelay = Random.Range(melodyPlayDelayMin, melodyPlayDelayMax);
		}
		else if(melodyPlayDelay > 0)
		{
			melodyPlayDelay -= Time.deltaTime;
		}

		// Generates a new melodyRest chance and a new break between checking for a new rest
		if(melodyRestDelay <= 0)
		{
			melodyRest = Random.Range(0f, 1f);
			melodyRestDelay = Random.Range(melodyRestDelayMin, melodyRestDelayMax);
		}
		else
		{
			melodyRestDelay -= Time.deltaTime;
		}

		if(songEndTimer < 0)
		{
			// AudioClip is generated with the number of samples being equal to all samples generated in the last number of second, equal to songDuration
			combinedSong = AudioClip.Create("MySong", totalSamplesMade, 1, 44100, false);
			float[] samples = new float[totalSamplesMade];
			int totalSamplesAdded = 0;
			
			// This for loop iterates through every generated clip in the last amount of time for songDuration
			for (int i = 0; i < generatedClips.Count; i++)
			{
				// The samples that need to be added to the combinedSong AudioClip is equal to the current clip's number of samples
				float[] samplesToAdd = new float[generatedClips[i].samples];

				// Apply all of the samples from the clip to samplesToAdd
				generatedClips[i].GetData(samplesToAdd, 0);
				
				for(int j = 0; j < samplesToAdd.Length; j++)
				{
					// Apply the current sample to add to the current position, affected by the number of samples added so far
					samples[j + totalSamplesAdded] = samplesToAdd[j];
				}

				// Increment the total samples added so far by the number of samples that were in the current clip
				totalSamplesAdded += generatedClips[i].samples;
			}

			// Apply all of the samples that were combined into the song AudioClip
			combinedSong.SetData(samples, 0);

			// Request that the user save the current combinedSong AudioClip
			SaveWavFile();

			// Reset variables for the next song to be recorded
			totalSamplesMade = 0;
			generatedClips.Clear();
			songEndTimer = songDuration;
		}
		else // If the songEndTimer is still above 0, the song is still being made, therefore...
		{
			// Reduce the timer by Time.deltaTime (the time it took to run the last frame, this is used to keep track of time the program has been running)
			songEndTimer -= Time.deltaTime;
		}
	}
    

	// CreateToneAudioClip() is used to create a sound wave using a passed frequency
    private AudioClip CreateToneAudioClip(float frequency, int seconds, bool halfSampleSize) {

		// sampleDurationSecs determines how long the wave will play for, value 2 is chosen as default so that the chord changes don't drown out the melody
        int sampleDurationSecs = seconds;

		// 44100 is default for music sampleRate
		int sampleRate = 44100;

		// sampleLength is the total number of samples needed for the audioClip
		int sampleLength = sampleRate * sampleDurationSecs;
        float maxValue = 1f / 4f;

		AudioClip audioClip;

		// Every sample is assigned data which is assigned to the local audioClip and then returned at the end of the function
		float[] samples;
		
		// Melody notes use halved sampleSize to play for half a second, since seconds needs to be an integer, halfing the sampleSize is another way of reducing time played for
		if (halfSampleSize)
		{
			// Halves the total samples for this audioClip
			samples = new float[sampleLength/2];
			audioClip = AudioClip.Create("tone", sampleLength / 2, 1, sampleRate, false);

			// Each sample is given the value required for outputting the sound that we are trying to produce
			for (var i = 0; i < sampleLength/2; i++)
			{
				float s = Mathf.Sin(2.0f * Mathf.PI * frequency * ((float)i / (float)sampleRate));
				float v = s * maxValue;
				samples[i] = v;
			}
		}
		else
		{
			// If not required to have half the sampleSize, this section of code runs instead, this section creates a chord
			samples = new float[sampleLength];
			audioClip = AudioClip.Create("chord", sampleLength, 1, sampleRate, false);
			for (var i = 0; i < sampleLength; i++)
			{
				float s = Mathf.Sin(2.0f * Mathf.PI * frequency * ((float)i / (float)sampleRate));
				float v = s * maxValue;
				samples[i] = v;
			}
		}

		// Assign all of the generated samples to local audioClip, then return
		audioClip.SetData(samples, 0);


		// The generated audioClip is added to the collection of clips made during this songDuration timer
		generatedClips.Add(audioClip);
		// Total samples are increased so that the combinedSong AudioClip will have the correct number of samples to use
		totalSamplesMade += audioClip.samples;


		// Return the generated audioClip
        return audioClip;
    }

	// Notes() is used to create and return the frequency for a passed note number
	private float Notes(float noteNum)
	{
		// This algorithm returns the frequency of the note number selected, the frequency used is Middle A as the base frequency for the algorithm
		float frequency = 440 * Mathf.Pow(2.0f, (noteNum/12.0f));
		return (frequency);
	}


	// playChord() is called after generating a chord and is used to play every note from the chord
	private void PlayChord(AudioClip[] chord)
	{
		// Iterates through the length of the function and plays each note simultaneously
		for (int i = 0; i < chord.Length; i++)
		{
			PlayOutAudio(chord[i]);
		}
	}


	// ChordGen() picks a random chord from a pre-defined array and returns an array of frequencies as a result
	private AudioClip[] ChordGen()
	{
		// The chords to choose from are put into a string, these chords are piano chords for C Major
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

		// Piano-C Major is the code in use here, other chords could be included however the program is designed for C Major music
		#region Piano-C Major
		// Statements check which chord was randomly chosen and chord is assigned each AudioClip before being returned
		if (chordToMake == "Cm")
		{
			chord = new AudioClip[1];

			// Notes() here takes the note number and returns a frequency for use in CreateToneAudioClip()
			// '+ (octave * 12)' is used to determine the current octave that the notes are being generated in
			// chord[] has been reduced to one element for use in saving data as multiple audio clips combined together doesn't work with chords
			float totalFreq = (Notes(-33 + (octave * 12)) + Notes(-29 + (octave * 12)) + Notes(-26 + (octave * 12)))/3;
			chord[0] = CreateToneAudioClip(totalFreq, chordDurationSeconds, false);
		}
		else if(chordToMake == "Fm")
		{
			chord = new AudioClip[1];

			float totalFreq = (Notes(-28 + (octave * 12)) + Notes(-36 + (octave * 12)) + Notes(-33 + (octave * 12))) / 3;
			chord[0] = CreateToneAudioClip(totalFreq, chordDurationSeconds, false);
		}
		else if (chordToMake == "Gm")
		{
			chord = new AudioClip[1];

			float totalFreq = (Notes(-26 + (octave * 12)) + Notes(-34 + (octave * 12)) + Notes(-31 + (octave * 12))) / 3;
			chord[0] = CreateToneAudioClip(totalFreq, chordDurationSeconds, false);
		}
		#endregion

		// Return the final chord array
		return chord;
	}


	// MelodyGen() creates the next note to be played in the melody using an array of possible note numbers
	private AudioClip MelodyGen()
	{
		// Notes are defined for melody to work with C Major chords
		int[] notesForMelody = new int[5] { -33, -31, -29, -26, -24 };

		// modifyNoteNumber is used to generate the next note of the melody as being slightly different from the last note
		// Without being drastically different
		int modifyNoteNumber = Random.Range(-2, 3);

		// To stop the next note of the melody being the same note, generate the next number until it is different
		while (modifyNoteNumber == 0)
		{
			modifyNoteNumber = Random.Range(-2, 3);
		}

		// Clamp the number between the minimum and maximum size of the array of notes to use
		lastMelodyNote = Mathf.Clamp(lastMelodyNote + modifyNoteNumber, 0, notesForMelody.Length-1);

		// Returns the AudioClip generated for the melody
		return CreateToneAudioClip(Notes(notesForMelody[lastMelodyNote] + ((octave + 1) * 12)), 1, true);
	}

    
#if UNITY_EDITOR
    private void SaveWavFile() {
		// Opens a save panel, asking the user to input a file path that will be used to save the current combinedSong variable
        string path = EditorUtility.SaveFilePanel("Where do you want the wav file to go?", "", "", "wav");
        SaveWavUtil.Save(path, combinedSong);
    }
#endif
}