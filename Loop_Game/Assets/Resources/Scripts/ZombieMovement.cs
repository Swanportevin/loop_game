using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ZombieAnimation))]
public class ZombieMovement : MonoBehaviour
{
    private float OldSpeed;
    private GameObject target;
    private NavMeshAgent agent;
    private ZombieAnimation zombieAnim;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        zombieAnim = GetComponent<ZombieAnimation>();
        agent.speed = Random.Range(1f, 10f);
        zombieAnim.AnimBasedOnSpeed(agent.speed);
    }

    void FixedUpdate()
    {
        if (target != null)
            agent.destination = target.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OldSpeed = agent.speed;
            agent.speed = 0;
            zombieAnim.StopAll();
            StopAllCoroutines();
            StartCoroutine(StartPunching());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            agent.speed = OldSpeed;
            zombieAnim.AnimBasedOnSpeed(agent.speed);
        }
    }

    private IEnumerator StartPunching()
    {
        agent.speed = 0;
        zombieAnim.SetPunch();
        yield return new WaitForSeconds(2f);
        StartCoroutine(StartPunching());
    }
}
