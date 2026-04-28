using UnityEngine;
using UnityEngine.AI;

public class KillerAI : MonoBehaviour
{
    public Transform player;

    [Header("Vision")]
    public float detectionRange = 12f;
    public float attackRange = 2f;
    public float fieldOfView = 60f;

    [Header("Hearing")]
    public float investigateThreshold = 0.5f;

    private NavMeshAgent agent;

    private Vector3 lastHeardPosition;
    private float lastSeenTime;
    private float memoryTime = 3f;

    private enum State { Patrol, Chase, Attack, Investigate }
    private State state;

    public Transform[] patrolPoints;
    private int patrolIndex;

    // Shared noise memory
    public static Vector3 globalNoisePosition;
    public static bool noiseHeard;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        state = State.Patrol;

        GoToNextPatrolPoint();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool seesPlayer = CanSeePlayer();

        // 🧠 Memory system (keeps chase even if briefly lost)
        if (seesPlayer)
        {
            lastSeenTime = Time.time;
        }

        bool stillRememberPlayer = (Time.time - lastSeenTime < memoryTime);

        // 🔊 Noise detection
        if (noiseHeard)
        {
            float noiseDist = Vector3.Distance(transform.position, globalNoisePosition);

            if (noiseDist <= detectionRange * 1.5f)
            {
                lastHeardPosition = globalNoisePosition;
                state = State.Investigate;
            }

            noiseHeard = false;
        }

        switch (state)
        {
            case State.Patrol:

                if (seesPlayer)
                {
                    state = State.Chase;
                }
                else if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    GoToNextPatrolPoint();
                }

                break;

            case State.Chase:

                agent.SetDestination(player.position);

                if (distance <= attackRange)
                {
                    state = State.Attack;
                }

                if (!seesPlayer && !stillRememberPlayer)
                {
                    state = State.Patrol;
                    GoToNextPatrolPoint();
                }

                break;

            case State.Attack:

                AttackPlayer();

                if (distance > attackRange)
                {
                    state = State.Chase;
                }

                break;

            case State.Investigate:

                agent.SetDestination(lastHeardPosition);

                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    state = State.Patrol;
                    GoToNextPatrolPoint();
                }

                if (CanSeePlayer())
                {
                    state = State.Chase;
                }

                break;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > fieldOfView)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, dirToPlayer, out hit, detectionRange))
        {
            if (hit.transform == player)
                return true;
        }

        return false;
    }

    void AttackPlayer()
    {
        Debug.Log("Player caught!");
        // Hook into player death system here
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[patrolIndex].position);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }
}