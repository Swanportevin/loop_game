using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PointWayfinderArrow : MonoBehaviour
{

    public GameObject playerOrigin;

    // Update is called once per frame
    void Update()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag("wayfindertarget");
        
        // Early exit if no target found
        if (targetObject == null)
            return;

        Vector3 _forward = playerOrigin.transform.forward;
        Vector2 forward = new(_forward.x, _forward.z);

        Vector3 _position_player = playerOrigin.transform.position;
        Vector2 position_player = new(_position_player.x, _position_player.z);

        Vector3 _position_target = targetObject.transform.position;
        Vector2 position_target = new(_position_target.x, _position_target.z);

        // Calculate direction TO the target, not just the target position
        Vector2 directionToTarget = (position_target - position_player).normalized;

        float angle = Vector2.SignedAngle(forward, directionToTarget);

        transform.rotation = Quaternion.Euler(0, 0, angle);

    }
}
