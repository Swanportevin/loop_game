using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableFlamethrower : MonoBehaviour
{
    public GameObject text;
    public GameObject image;
    public SwitchWeapons switchWeapons;

    public void Enable()
    {
        text.SetActive(true);
        image.SetActive(true);
        switchWeapons.flamethrowerEnabled = true;
    }
}
