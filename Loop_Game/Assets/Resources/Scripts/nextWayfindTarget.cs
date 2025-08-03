using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextWayfindTarget : MonoBehaviour
{
    public GameObject target;
    public void SetNextWayfindTarget()
    {
        Debug.Log($"Next Target {target.transform.name}");
        if (target != null && transform.tag == "wayfindertarget")
        {
            Debug.Log($"Target Set successfully");

            target.tag = "wayfindertarget";
            transform.tag = "Untagged";
        }
    }
}
