using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfOverlapping : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("forest"))
        {
            if (gameObject.GetInstanceID() > other.gameObject.GetInstanceID())
            {
                Destroy(gameObject);
            }
        }
    }
}
