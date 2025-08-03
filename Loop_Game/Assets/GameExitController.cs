using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExitController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check for mouse click to capture cursor
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            CaptureMouse();
        }
        
        // Check for Escape key to uncapture cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Check if Shift is also pressed for quit functionality
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                QuitGame();
            }
            else
            {
                UncaptureMouse();
            }
        }
    }
    
    private void CaptureMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void UncaptureMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
