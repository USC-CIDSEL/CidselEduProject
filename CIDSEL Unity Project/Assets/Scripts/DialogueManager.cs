using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private Queue<string> names;
    private Queue<float> pauseStamps;
    private bool DialogueStarted = false;
    private bool ButtonJustPressed = false;
    private bool directorIsPlaying = false;
    private float ButtonDelayTime;
    private PlayableDirector director;
    public bool playTrigger;
    public DialogueTrigger trigger;
    //public TimelineAsset choiceATimeline;
    //public TimelineAsset ChoiceBTimeline;
    public Text charNameText;
    public Text dialogueText;
    public Text choiceAText;
    public Text choiceBText;
    public string mapToWarpTo;


    // Start is called before the first frame update
    void Start()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;

        //initialise Queue objects and director.

        director = GetComponent<PlayableDirector>();
        if (!director)
        {
            Debug.Log("Director is Null!");
        }
        director.Stop();
        if (trigger && playTrigger)
        {
            Debug.Log("Trigger!");
            trigger.TriggerDialogue();
        }

    }

    private void Update()
    {
        CheckDelay();
        //If The dialogue has started, a timeline isn't playing and wasn't just advanced, then on submit continue dialogue.
        if (DialogueStarted && !directorIsPlaying && Input.GetButtonDown("Submit") && !ButtonJustPressed)
        {
            ButtonJustPressed = true;
            ButtonDelayTime = 0.2f;
            DisplayNextSentence(false);
        }
    }

    public void StartDialogue(DialogueObject dialogue, TimelineAsset timeline)
    {
        Debug.Log("StartDialogue called.");
        sentences = new Queue<string>();
        names = new Queue<string>();
        pauseStamps = new Queue<float>();
        director.Stop();
        director.playableAsset = timeline;
        director.RebuildGraph();
        director.time = 0.0;
        names.Clear();
        sentences.Clear();
        pauseStamps.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        foreach (string name in dialogue.names)
        {
            names.Enqueue(name);
        }
        foreach (float timestamp in dialogue.timestamps)
        {
            pauseStamps.Enqueue(timestamp);
        }
        DialogueStarted = true;
        DisplayNextSentence(true);

    }

    public void DisplayNextSentence(bool isfirstLine)
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            EndScene();
        }
        else
        {
            string name = names.Dequeue();
            string sentence = sentences.Dequeue();
            float timestamp = pauseStamps.Dequeue();
            if (isfirstLine)
            {
                PlayTimeline();
            }
            else
            {
                ResumeTimeline();
            }
            Debug.Log("DisplayNextSentence called - " + sentences.Count + " sentences ready");
            charNameText.text = name;
            dialogueText.text = sentence;
            Invoke("PauseTimeline", timestamp); //Waits for the timestamp (and by extension, the animation to play out) before pausing the timeline.
            Debug.Log(timestamp);
            if (names.Peek().Equals("<CHOICE>"))
            {
                //EndDialogue();
                string[] choiceTexts = sentences.Peek().Split('^');
                choiceAText.text = choiceTexts[0];
                choiceBText.text = choiceTexts[1];
                Debug.Log("Split strings into: " + choiceTexts[0] + " ||| " + choiceTexts[1]);
                names.Dequeue();
                sentences.Dequeue();
                pauseStamps.Dequeue();
                Invoke("EndDialogue", timestamp); //Waits for the timestamp (and by extension, the animation to play out) before stopping the dialogue.
            }
        }
    }

    void EndDialogue()
    {
        DialogueStarted = false;
        Debug.Log("End of conversation.");
        director.Stop();
    }

    public void EndScene()
    {
        SceneManager.LoadScene(mapToWarpTo);
        Debug.Log("Went back to map!");
        EndDialogue();
    }

    void CheckDelay()
    {
        if (ButtonJustPressed && ButtonDelayTime > 0)
        {
            ButtonDelayTime -= Time.deltaTime;
        }
        if (ButtonDelayTime < 0)
        {
            ButtonDelayTime = 0;
            ButtonJustPressed = false;
        }
    }

    public void SetPersistentData(int sceneNumber)
    {
        switch (sceneNumber)
        {
            case 1:
                PersistantData.scn1Choice = true;
                break;
            case 2:
                PersistantData.scn2Choice = true;
                break;
            case 3:
                PersistantData.scn3Choice = true;
                break;
            case 4:
                PersistantData.scn4Choice = true;
                break;
            case 5:
                PersistantData.scn5Choice = true;
                break;
            case 6:
                PersistantData.scn6Choice = true;
                break;
            case 7:
                PersistantData.scn7Choice = true;
                break;
            default:
                break;
        }
    }

    public void SetIsBitten(bool isBitten)
    {
        PersistantData.isBitten = isBitten;
    }

    #region Timeline Controls
    void PlayTimeline()
    {
        Debug.Log("Timeline started!");
        director.Play();
        directorIsPlaying = true;
    }

    void ResumeTimeline()
    {
        Debug.Log("Timeline resumed!");
        director.Resume();
        directorIsPlaying = true;
    }

    void PauseTimeline()
    {
        director.Pause();
        Debug.Log("Timeline Paused!");
        directorIsPlaying = false;
    }
    #endregion
}


// ~Sources~
// Playable Director info: https://www.youtube.com/watch?v=cmExSYI2yd0
// Button Delay: https://answers.unity.com/questions/679948/how-to-make-a-delay-on-key-input.html
// Dialogue Stuff: Brackeys
