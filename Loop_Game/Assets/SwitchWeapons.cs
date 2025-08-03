using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapons : MonoBehaviour
{
    
    public bool flamethrowerEnabled = false;
    public CamShooting shoting;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            shoting.useFlamethrower = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && flamethrowerEnabled)
        {
            shoting.useFlamethrower = true;
        }

    }
}
