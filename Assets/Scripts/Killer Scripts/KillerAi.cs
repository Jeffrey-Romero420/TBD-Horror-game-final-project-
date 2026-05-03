using UnityEngine;
using UnityEngine.AI;

public class KillerAI : MonoBehaviour
{
    public Transform player;
    private CrouchScript crouchScript; // ✅ reads crouch state for vision

    [Header("Startup")]
    public float initialCalmTime = 3f;
    private float startTime;
    private bool aiActive;

    [Header("Vision")]
    public float detectionRange = 12f;
    public float fieldOfView = 60f;         // total FOV in degrees
    public float losePlayerTime = 3f;       // seconds without sight before giving up chase
    public float crouchDetectionRange = 6f;   // ✅ detection range is reduced when player crouches
    public float crouchFOV = 30f;             // ✅ FOV is also reduced when player crouches
    private float lastSeenPlayerTime = -1f;
    private Vector3 lastKnownPlayerPos;     // where the killer last saw the player

    [Header("Attack")]
    public float attackRange = 2f;
    private bool hasAttacked = false;

    [Header("Search")]
    public float waitTimeAtPoint = 2f;
    public float lookAroundSpeed = 120f;

    private float waitTimer;
    private bool isWaiting;

    private NavMeshAgent agent;
    private Vector3 lastHeardPosition;

    private enum State { Patrol, Investigate, Chase, Attack }
    private State state;

    public Transform[] patrolPoints;
    private int patrolIndex;

    [Header("Aggression — tune these to balance difficulty")]
    public float killerMoveSpeed = 4f;        // NavMesh agent speed
    public float killerAcceleration = 8f;     // how quickly killer reaches top speed
    public float noiseHearingRange = 15f;     // ✅ replaces detectionRange * 2 for noise
    public float losePlayerDistance = 18f;    // if player gets this far away, give up chase faster

    // Noise system
    public static Vector3 globalNoisePosition;
    public static bool noiseHeard;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = killerMoveSpeed;
        agent.acceleration = killerAcceleration;
        state = State.Patrol;
        startTime = Time.time;
        aiActive = false;
        agent.ResetPath();
        Invoke(nameof(BeginPatrol), 0.2f);
        noiseHeard = false;

        // Cache crouch script from player
        if (player != null)
            // ✅ Check root first, then children (handles any player hierarchy)
            crouchScript = player.GetComponent<CrouchScript>();
        if (crouchScript == null)
            crouchScript = player.GetComponentInChildren<CrouchScript>();

        if (crouchScript == null)
            Debug.LogWarning("KillerAI: CrouchScript not found on player!");
    }

    void Update()
    {
        // Startup delay
        if (!aiActive)
        {
            if (Time.time >= startTime + initialCalmTime)
                aiActive = true;
            return;
        }

        if (agent == null || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool seesPlayer = CanSeePlayer();

        // Vision — track last known position and handle sight loss
        if (seesPlayer)
        {
            lastKnownPlayerPos = player.position;
            lastSeenPlayerTime = Time.time;

            if (state != State.Attack)
                state = State.Chase;
        }
        else if (state == State.Chase)
        {
            float timeLost = Time.time - lastSeenPlayerTime;

            // ✅ Give up chase early if player is far away AND out of sight
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            bool farAway = distToPlayer > losePlayerDistance;

            if (timeLost >= losePlayerTime || (farAway && timeLost >= losePlayerTime * 0.4f))
            {
                // Lost the player — investigate last known position then give up
                lastHeardPosition = lastKnownPlayerPos;
                state = State.Investigate;
                Debug.Log("Lost player — investigating last known position");
            }
            // Within timeout: Chase() will move toward lastKnownPlayerPos
        }

        // ✅ Attack triggers purely on distance — killer grabs player if close enough
        // regardless of vision state, so the player can't dodge by stepping out of FOV
        if (distance <= attackRange && state != State.Attack)
        {
            state = State.Attack;
        }

        // Noise handling — only triggers if not already chasing or attacking
        // ✅ Ignore noise if player is safely hidden in a wardrobe
        if (HidingSpot.playerHiding && HidingSpot.isSafeHide)
            noiseHeard = false;

        if (noiseHeard)
        {
            float noiseDist = Vector3.Distance(transform.position, globalNoisePosition);

            if (noiseDist <= noiseHearingRange)
            {
                lastHeardPosition = globalNoisePosition;

                if (!seesPlayer && state != State.Chase && state != State.Attack)
                {
                    state = State.Investigate;
                }
            }

            noiseHeard = false;
        }

        switch (state)
        {
            case State.Patrol: Patrol(); break;
            case State.Investigate: Investigate(); break;
            case State.Chase: Chase(); break;
            case State.Attack: Attack(distance); break;
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
                agent.isStopped = true;
            }

            waitTimer -= Time.deltaTime;
            transform.Rotate(0, lookAroundSpeed * Time.deltaTime, 0);

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                agent.isStopped = false;
                GoToNextPatrolPoint();
            }
        }
    }

    void Investigate()
    {
        agent.isStopped = false;
        agent.SetDestination(lastHeardPosition);

        // Spotted player while investigating -> chase
        if (CanSeePlayer())
        {
            state = State.Chase;
            return;
        }

        // Reached the spot, nothing found -> resume patrol
        if (!agent.pathPending && agent.remainingDistance <= 1.5f)
        {
            state = State.Patrol;
            GoToNextPatrolPoint();
        }
    }

    void Chase()
    {
        agent.isStopped = false;

        float dist = Vector3.Distance(transform.position, player.position);

        // ✅ If player is very close, always chase them directly (prevents getting stuck on lastKnownPos)
        if (CanSeePlayer() || dist <= attackRange * 3f)
            agent.SetDestination(player.position);
        else
            agent.SetDestination(lastKnownPlayerPos);
    }

    void Attack(float distance)
    {
        agent.ResetPath();

        if (!hasAttacked)
        {
            hasAttacked = true;
            AttackPlayer();
        }

        // Player backed away — resume chase
        if (distance > attackRange + 1f)
        {
            hasAttacked = false;
            state = State.Chase;
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        // ✅ Player is in a safe hiding spot (wardrobe) — killer cannot see them
        if (HidingSpot.playerHiding && HidingSpot.isSafeHide)
            return false;

        // ✅ Check if player is crouching and adjust detection + ray target
        bool crouching = crouchScript != null && crouchScript.isCrouching;
        float effectiveRange = crouching ? crouchDetectionRange : detectionRange;
        float effectiveFOV = crouching ? crouchFOV : fieldOfView;

        // ✅ Ray aims lower when crouched so it actually hits the player body
        float targetHeight = crouching ? 0.3f : 1.0f;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 target = player.position + Vector3.up * targetHeight;
        Vector3 dir = (target - origin).normalized;

        float dist = Vector3.Distance(origin, target);

        if (dist > effectiveRange) return false;

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > effectiveFOV / 2f) return false;

        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, effectiveRange))
        {
            Debug.DrawRay(origin, dir * effectiveRange, crouching ? Color.yellow : Color.red);

            if (hit.transform == player || hit.transform.IsChildOf(player))
                return true;
        }

        return false;
    }

    void AttackPlayer()
    {
        Debug.Log("ATTACK");

        PlayerHealth health = player.GetComponentInParent<PlayerHealth>();

        if (health != null && !health.isDead)
            health.Die();
        else
            Debug.LogError("PlayerHealth missing!");
    }

    void BeginPatrol()
    {
        GoToNextPatrolPoint();
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Vector3 target = patrolPoints[patrolIndex].position;
        Vector3 offset = Random.insideUnitSphere * 0.5f;
        offset.y = 0;

        agent.SetDestination(target + offset);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }
}