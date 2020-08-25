using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Bot : MonoBehaviour
{
    
    public Transform m_Target;

    [Header("Wander")]
    public float m_WanderRadius = 10.0f;
    public float m_wanderDistance = 20.0f;
    public float m_WanderJitter = 1.0f;
    private Vector3 m_WanderTarget = Vector3.zero;

    private float WanderRandom => Random.Range(-1.0f, 1.0f) * m_WanderJitter;

    [Header("Hiding")]
    public float m_HidingOffset = 3.0f;
    private GameObject[] m_HidingSpots;

    [Header("Path Follow")]
    public Path m_Path;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Animator.SetFloat("Speed", 10);
    }

    public void Update()
    {
        float speed = m_Agent.velocity.magnitude;
        m_Animator.SetFloat("Speed", speed);

        PathFollow();
    }

    private void Start()
    {
        m_HidingSpots = GameObject.FindGameObjectsWithTag("HidingSpot");
    }

    private void PathFollow()
    {
        var node = m_Path.GetNode();
        Seek(node);

        var radius = m_Path.GetRadius();
        if (Vector3.Distance(transform.position, node) < radius)
        {
            m_Path.NextNode();
        }
    }



    private void Hide()
    {
        float chosenSpotDistance = Mathf.Infinity;
        Vector3 chosenSpotPosition = Vector3.zero;

        foreach (var hidingSpot in m_HidingSpots)
        {
            var direction = hidingSpot.transform.position - m_Target.transform.position;
            var position = hidingSpot.transform.position + direction.normalized * m_HidingOffset;
            var distance = Vector3.Distance(transform.position, position);

            if (distance < chosenSpotDistance)
            {
                chosenSpotDistance = distance;
                chosenSpotPosition = position;
            }
        }

        Seek(chosenSpotPosition);
    }

    private void Seek(Vector3 position)
    {
        m_Agent.SetDestination(position);    
    }

    private void Flee(Vector3 position)
    {
        var fleeVector = position - transform.position;
        m_Agent.SetDestination(transform.position - fleeVector);
    }

    private void Wander()
    {
        m_WanderTarget += new Vector3(WanderRandom, 0.0f, WanderRandom);

        m_WanderTarget.Normalize();
        m_WanderTarget *= m_WanderRadius;

        var targetLocal = m_WanderTarget + new Vector3(0, 0, m_wanderDistance);
        var targetWorld = transform.InverseTransformVector(targetLocal);

        Seek(transform.position + targetWorld);
    }
}
