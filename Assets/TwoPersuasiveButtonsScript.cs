using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class TwoPersuasiveButtonsScript : MonoBehaviour
{
	public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;
	public AudioSource BadTimes;
	
	public KMSelectable[] Choices;
	public TextMesh[] Selection;
	public AudioClip[] SFX;
	
	string BaseLine;
	string[][] Outcome = new string[5][]{
		new string[5] {"A", "B", "B", "A", "A"},
		new string[5] {"A", "B", "A", "B", "B"},
		new string[5] {"B", "B", "B", "A", "A"},
		new string[5] {"A", "B", "A", "B", "A"},
		new string[5] {"B", "B", "A", "B", "A"}
	};
	
	int[] SelectedNumber = {0,0};
	int Stage = 0;
	int Score = 0;
	
	//Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;
	
	void Awake()
    {
		moduleId = moduleIdCounter++;
        for (int b = 0; b < 2; b++)
        {
            int Numbered = b;
            Choices[Numbered].OnInteract += delegate
            {
                Press(Numbered);
				return false;
            };
        }
    }

	void Start()
	{
		Module.OnActivate += TextGenerate;
	}
	
	void TextGenerate()
	{
		string[] TextSelection = {"Push Me", "Click Me", "Select Me", "Tap Me", "Press Me"};
		for (int x = 0; x < 2; x++)
		{
			SelectedNumber[x] = UnityEngine.Random.Range(0,5);
			Selection[x].text = TextSelection[SelectedNumber[x]];
		}
		BaseLine = Outcome[SelectedNumber[0]][SelectedNumber[1]];
		
		Debug.LogFormat("[Two Persuasive Buttons #{0}] {1} {2} {3}", moduleId, Selection[0].text, Outcome[SelectedNumber[0]][SelectedNumber[1]] == "A" ? "<-" : "->", Selection[1].text);
	}
	
	void Press(int Numbered)
    {
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);
		Choices[Numbered].AddInteractionPunch(.2f);
		if (!ModuleSolved && !BadTimes.isPlaying)
		{
			Debug.LogFormat("[Two Persuasive Buttons #{0}] You pressed {1}", moduleId, Numbered == 0 ? "<-" : "->");
			if (Numbered == 0)
			{
				if (BaseLine == "A")
				{
					Score++;
				}
				Stage++;
				if (Stage == 3)
				{
					if (Score == 3)
					{
						Debug.LogFormat("[Two Persuasive Buttons #{0}] You did it correctly. Module solved.", moduleId);
						Module.HandlePass();
						ModuleSolved = true;
						Selection[0].text = "NICE";
						Selection[1].text = "JOB";
						Audio.PlaySoundAtTransform(SFX[0].name, transform);
					}
					
					else
					{
						StartCoroutine(SadTimes());
					}
				}
				else
				{
					TextGenerate();
				}
			}
			
			else
			{
				if (BaseLine == "B")
				{
					Score++;
				}
				Stage++;
				if (Stage == 3)
				{
					if (Score == 3)
					{
						Debug.LogFormat("[Two Persuasive Buttons #{0}] You did it correctly. Module solved.", moduleId);
						Module.HandlePass();
						ModuleSolved = true;
						Selection[0].text = "NICE";
						Selection[1].text = "JOB";
						Audio.PlaySoundAtTransform(SFX[0].name, transform);
					}
					
					else
					{
						StartCoroutine(SadTimes());
					}
				}
				else
				{
					TextGenerate();
				}
			}
		}
    }
	
	IEnumerator SadTimes()
	{
		Debug.LogFormat("[Two Persuasive Buttons #{0}] You did something wrong. Module performed a reset.", moduleId);
		Selection[0].text = "WHY!?";
		Selection[1].text = "WHY!?";
		BadTimes.clip = SFX[1];
		BadTimes.Play();
		while (BadTimes.isPlaying)
		{
			yield return new WaitForSecondsRealtime(0.1f);
		}
		Module.HandleStrike();
		Stage = 0;
		Score = 0;
		TextGenerate();
	}
	
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use the command !{0} left/right to press the corresponding button";
    #pragma warning restore 414
	
    IEnumerator ProcessTwitchCommand(string command)
    {
		if (BadTimes.isPlaying)
		{
			yield return "sendtochaterror Module is performing a strike. The command was not processed.";
			yield break;
		}
		
		if (Regex.IsMatch(command, @"^\s*left\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*l\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;	
			yield return "strike";
            Choices[0].OnInteract();
        }
		
        if (Regex.IsMatch(command, @"^\s*right\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*r\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
			yield return "strike";
            Choices[1].OnInteract();
        }
	}
}
