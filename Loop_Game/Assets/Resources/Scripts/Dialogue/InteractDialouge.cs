using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDialouge : MonoBehaviour
{

    public GameObject container;

    public void Interact()
    {
        String original_name = transform.parent.name.Replace("(Clone)", "");
        Debug.Log($"Looking for {original_name}");
        for (int i = 0; i < container.transform.childCount; i++)
        {
            GameObject child = container.transform.GetChild(i).gameObject;
            if (child.name == original_name)
            {
                child.GetComponent<PlayDialogue>().Play();
            }
        }
    }
}
