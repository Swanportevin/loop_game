using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class despawn : MonoBehaviour
{

    public float despawnDuration = 5f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, despawnDuration);   
    }
}
