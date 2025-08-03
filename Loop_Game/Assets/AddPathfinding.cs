using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPathfinding : MonoBehaviour
{
    public bool startPathfinding = false;
    public GameObject Wayfinder;
    public GameObject Minimap;
    public GameObject ui_Wayfinder;
    public GameObject ui_Minimap;
    
    public GameManager gameManager;

    public int minCount = 1;


    public void startPathfindingSet()
    {
        startPathfinding = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startPathfinding)
        {
            if (gameManager.Score >= minCount)
            {
                Wayfinder.SetActive(true);
                Minimap.SetActive(true);
                ui_Wayfinder.SetActive(true);
                ui_Minimap.SetActive(true);
            }
        }
    }
}
