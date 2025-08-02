using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public GameObject DialougeManager;
    
    [Header("Dialogue Lines")]
    [TextArea(2, 5)]
    public string[] dialogueLines;
    
    [Header("Playback Settings")]
    public bool loopDialogue = true; // Whether to loop back to the beginning when all lines are complete

    public UnityEvent OnDialogueIsCompleted;

    // Private variables
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isCurrentLineComplete = true; // Tracks if the current line has finished displaying
    private Dialouge dialogueComponent;
    
    void Start()
    {
        // Cache the dialogue component reference
        if (DialougeManager != null)
        {
            dialogueComponent = DialougeManager.GetComponent<Dialouge>();
        }
        
        // Subscribe to dialogue finished event
        Dialouge.DialogueFinishedEvent += OnDialogueFinished;
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        Dialouge.DialogueFinishedEvent -= OnDialogueFinished;
    }
    
    /// <summary>
    /// Starts playing the dialogue sequence or advances to the next line
    /// </summary>
    public void Play()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogWarning($"No dialogue lines set for {transform.name}");
            return;
        }
        
        if (dialogueComponent == null)
        {
            Debug.LogError($"Dialogue Manager not assigned or Dialouge component not found for {transform.name}");
            return;
        }
        
        // If no dialogue is active, start from the beginning
        if (!isDialogueActive)
        {
            currentLineIndex = 0;
            isDialogueActive = true;
            isCurrentLineComplete = false;
            PlayCurrentLine();
            return;
        }
        
        // If dialogue is active but current line isn't complete, ignore the call
        if (!isCurrentLineComplete)
        {
            return;
        }
        
        // Advance to next line
        currentLineIndex++;
        
        // Check if we've reached the end
        if (currentLineIndex >= dialogueLines.Length)
        {

            OnDialogueIsCompleted.Invoke();
            if (loopDialogue)
            {
                // Loop back to the beginning
                currentLineIndex = 0;
                Debug.Log($"Dialogue sequence looped for {transform.name}");
            }
            else
            {
                // End the dialogue sequence
                isDialogueActive = false;
                Debug.Log($"Dialogue sequence completed for {transform.name}");
                return;
            }
        }
        
        // Play the current line
        isCurrentLineComplete = false;
        PlayCurrentLine();
    }
    
    /// <summary>
    /// Plays the current dialogue line
    /// </summary>
    private void PlayCurrentLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            string currentLine = dialogueLines[currentLineIndex];
            
            // Replace placeholder with object name if it contains {name}
            currentLine = currentLine.Replace("{name}", transform.name);
            
            dialogueComponent.DisplayMessage(currentLine);
        }
    }
    
    /// <summary>
    /// Called when a dialogue line finishes displaying
    /// </summary>
    private void OnDialogueFinished()
    {
        // Mark the current line as complete, allowing the next Play() call to advance
        isCurrentLineComplete = true;
    }
    
    /// <summary>
    /// Stops the current dialogue sequence
    /// </summary>
    public void StopDialogue()
    {
        isDialogueActive = false;
        isCurrentLineComplete = true;
        
        if (dialogueComponent != null)
        {
            dialogueComponent.ClearDialogue();
        }
    }
    
    /// <summary>
    /// Restarts the dialogue sequence from the beginning
    /// </summary>
    public void RestartDialogue()
    {
        currentLineIndex = 0;
        isDialogueActive = false;
        isCurrentLineComplete = true;
        
        if (dialogueComponent != null)
        {
            dialogueComponent.ClearDialogue();
        }
    }
    
    /// <summary>
    /// Checks if dialogue is currently active
    /// </summary>
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
    
    /// <summary>
    /// Checks if the current line has finished displaying
    /// </summary>
    public bool IsCurrentLineComplete()
    {
        return isCurrentLineComplete;
    }
    
    /// <summary>
    /// Gets the current dialogue line index
    /// </summary>
    public int GetCurrentLineIndex()
    {
        return currentLineIndex;
    }
}
