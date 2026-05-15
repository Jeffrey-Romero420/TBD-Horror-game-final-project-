using UnityEngine;
using UnityEngine.AI;

public class KillerOpensDoor : MonoBehaviour
{
    public float checkDistance = 2f;
    public float stopDistance = 1.5f;

    private NavMeshAgent agent;
    private bool isOpeningDoor = false;  // ✅ prevents stacking multiple Invokes

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!isOpeningDoor)
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
                    isOpeningDoor = true;
                    agent.isStopped = true;
                    door.OpenDoor();
                    Invoke(nameof(ResumeMovement), 0.7f);
                }
            }
        }
    }

    void ResumeMovement()
    {
        agent.isStopped = false;
        isOpeningDoor = false;
    }
}