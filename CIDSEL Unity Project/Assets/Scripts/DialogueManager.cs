using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
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
    //private TimelineAsset timeline;
    private PlayableDirector director;
    public Text charNameText;
    public Text dialogueText;
    private float currentStamp;

    // Start is called before the first frame update
    void Start()
    {
        //initialise Queue objects and director.
        sentences = new Queue<string>();
        names = new Queue<string>();
        pauseStamps = new Queue<float>();
        director = GetComponent<PlayableDirector>();
        if (!director)
        {
            Debug.Log("Director is Null!");
        }
        director.Stop();
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

    public void StartDialogue(DialogueObject dialogue)
    {
        Debug.Log("StartDialogue called.");
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

    public void DisplayNextSentence(bool firstLine)
    {
        if (firstLine)
        {
            PlayTimeline();
        }
        else
        {
            ResumeTimeline();
        }
        Debug.Log("DisplayNextSentence called - " + sentences.Count + " sentences ready");
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string name = names.Dequeue();
        string sentence = sentences.Dequeue();
        float timestamp = pauseStamps.Dequeue();
        charNameText.text = name;
        dialogueText.text = sentence;
        Invoke("PauseTimeline", timestamp);
        Debug.Log(timestamp);
    }
    void EndDialogue()
    {
        DialogueStarted = false;
        Debug.Log("End of conversation.");
        director.Stop();
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
}


// ~Sources~
// Playable Director info: https://www.youtube.com/watch?v=cmExSYI2yd0
// Button Delay: https://answers.unity.com/questions/679948/how-to-make-a-delay-on-key-input.html
// Dialogue Stuff: Brackeys
