using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    public string sceneName = "SampleScene";
    
    // Start is called before the first frame update
    public void StartGame()
    {
        // Load the specified scene
        SceneManager.LoadScene(sceneName);
    }
}
