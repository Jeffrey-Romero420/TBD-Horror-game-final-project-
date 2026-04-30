using UnityEngine;
using UnityEngine.AI;

public class KillerOpensDoor : MonoBehaviour

{
    public float checkDistance = 2f;
    public float stopDistance = 1.5f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        CheckForDoor();
    }

    void CheckForDoor()
    {
        RaycastHit hit;

        Vector3 origin = transform.position + Vector3.up * 1.5f;

        if (Physics.Raycast(origin, transform.forward, out hit, checkDistance))
        {
            Door door = hit.transform.GetComponentInParent<Door>();

            if (door != null && !door.isOpen)
            {
                float distance = Vector3.Distance(transform.position, door.transform.position);

                if (distance <= stopDistance)
                {
                    agent.isStopped = true;   // stop at door
                    door.OpenDoor();          // open it
                    Invoke(nameof(ResumeMovement), 0.7f); // wait then move
                }
            }
        }
    }

    void ResumeMovement()
    {
        agent.isStopped = false;
    }
}