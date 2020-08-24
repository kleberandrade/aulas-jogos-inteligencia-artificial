using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
public class Bot : MonoBehaviour
{
    
    private NavMeshAgent m_Agent;
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

    [Header("Stamina")]
    public float m_Stamina = 100.0f;
    public float m_DecreaseStaminaPerTime = 5.0f;
    private float m_RecoverTime = 1.0f;

    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_HidingSpots = GameObject.FindGameObjectsWithTag("HidingSpot");
        InvokeRepeating("Recover", m_RecoverTime, m_RecoverTime);
    }

    private void Recover()
    {
        m_Stamina = Mathf.Clamp(++m_Stamina, 0, 100);
    }

    private void Update()
    {
        if (m_Stamina < 5.0f)
            Hide();
        else if (m_Stamina < 40.0f)
            Flee(m_Target.position);
        else
            Seek(m_Target.position);

        //Hide();
        //Seek(m_Target.position);
        //Flee(m_Target.position);
        //Wander();
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
        m_Stamina -= m_DecreaseStaminaPerTime * Time.deltaTime;

        m_Agent.SetDestination(position);    
    }

    private void Flee(Vector3 position)
    {
        m_Stamina -= m_DecreaseStaminaPerTime * Time.deltaTime;

        var fleeVector = position - transform.position;
        m_Agent.SetDestination(transform.position - fleeVector);
    }

    private void Wander()
    {
        m_Stamina -= m_DecreaseStaminaPerTime * Time.deltaTime;

        m_WanderTarget += new Vector3(WanderRandom, 0.0f, WanderRandom);

        m_WanderTarget.Normalize();
        m_WanderTarget *= m_WanderRadius;

        var targetLocal = m_WanderTarget + new Vector3(0, 0, m_wanderDistance);
        var targetWorld = transform.InverseTransformVector(targetLocal);

        Seek(transform.position + targetWorld);
    }
}
