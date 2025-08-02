using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Dialouge : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueTextBox; // The text box to write to
    public GameObject dialoguePanel;
    
    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f; // Speed of typing animation (seconds per character)
    public bool useTypingAnimation = true; // Whether to use typing animation
    public float hideDelayAfterCompletion = 2f; // Delay before hiding panel after dialogue completes
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnDialogueFinished; // Event triggered when dialogue is finished
    
    // Private variables
    private Coroutine typingCoroutine;
    private Coroutine hidePanelCoroutine;
    private bool isTyping = false;
    
    // Action event for code-based subscribers
    public static event Action DialogueFinishedEvent;
    
    void Start()
    {
        // Initialize dialogue text box to be empty
        if (dialogueTextBox != null)
        {
            dialogueTextBox.text = "";
        }
        
        // Hide dialogue panel initially
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Displays a message in the dialogue text box
    /// </summary>
    /// <param name="message">The message to display</param>
    public void DisplayMessage(string message)
    {
        if (dialogueTextBox == null)
        {
            Debug.LogError("Dialogue text box is not assigned!");
            return;
        }
        
        // Show dialogue panel when starting dialogue
        ShowDialoguePanel();
        
        // Stop any current typing animation
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        // Stop any pending hide panel coroutine
        if (hidePanelCoroutine != null)
        {
            StopCoroutine(hidePanelCoroutine);
            hidePanelCoroutine = null;
        }
        
        if (useTypingAnimation)
        {
            typingCoroutine = StartCoroutine(TypeMessage(message));
        }
        else
        {
            dialogueTextBox.text = message;
            OnDialogueComplete();
        }
    }
    
    /// <summary>
    /// Coroutine for typing animation
    /// </summary>
    /// <param name="message">The message to type</param>
    /// <returns></returns>
    private IEnumerator TypeMessage(string message)
    {
        isTyping = true;
        dialogueTextBox.text = "";
        
        foreach (char letter in message.ToCharArray())
        {
            dialogueTextBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        OnDialogueComplete();
    }
    
    /// <summary>
    /// Called when dialogue is finished displaying
    /// </summary>
    private void OnDialogueComplete()
    {
        // Trigger Unity Event
        OnDialogueFinished?.Invoke();
        
        // Trigger static event for code subscribers
        DialogueFinishedEvent?.Invoke();
        
        Debug.Log("Dialogue finished displaying");
        
        // Start countdown to hide panel
        if (hidePanelCoroutine != null)
        {
            StopCoroutine(hidePanelCoroutine);
        }
        hidePanelCoroutine = StartCoroutine(HidePanelAfterDelay());
    }
    
    /// <summary>
    /// Skips the typing animation and shows the full message immediately
    /// </summary>
    public void SkipTyping()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            // This will be set by the DisplayMessage method when it detects the current message
            isTyping = false;
        }
    }
    
    /// <summary>
    /// Clears the dialogue text box
    /// </summary>
    public void ClearDialogue()
    {
        if (dialogueTextBox != null)
        {
            dialogueTextBox.text = "";
        }
        
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }
        
        // Stop any pending hide panel coroutine and hide immediately
        if (hidePanelCoroutine != null)
        {
            StopCoroutine(hidePanelCoroutine);
            hidePanelCoroutine = null;
        }
        
        HideDialoguePanel();
    }
    
    /// <summary>
    /// Checks if dialogue is currently being typed
    /// </summary>
    /// <returns>True if currently typing, false otherwise</returns>
    public bool IsTyping()
    {
        return isTyping;
    }
    
    /// <summary>
    /// Shows the dialogue panel
    /// </summary>
    private void ShowDialoguePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
    }
    
    /// <summary>
    /// Hides the dialogue panel
    /// </summary>
    private void HideDialoguePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Coroutine to hide the panel after a delay
    /// </summary>
    private IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(hideDelayAfterCompletion);
        HideDialoguePanel();
        hidePanelCoroutine = null;
    }
    
    /// <summary>
    /// Manually show the dialogue panel (useful for external control)
    /// </summary>
    public void ShowPanel()
    {
        ShowDialoguePanel();
        
        // Stop any pending hide operation
        if (hidePanelCoroutine != null)
        {
            StopCoroutine(hidePanelCoroutine);
            hidePanelCoroutine = null;
        }
    }
    
    /// <summary>
    /// Manually hide the dialogue panel (useful for external control)
    /// </summary>
    public void HidePanel()
    {
        // Stop any pending hide operation
        if (hidePanelCoroutine != null)
        {
            StopCoroutine(hidePanelCoroutine);
            hidePanelCoroutine = null;
        }
        
        HideDialoguePanel();
    }
}
