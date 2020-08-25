using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_SpeedMove = 10.0f;
    public float m_SpeedRotate = 180;

    private Rigidbody m_Body;
    private Animator m_Animator;

    private void Awake()
    {
        m_Body = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, horizontal * m_SpeedRotate * Time.deltaTime);
        //transform.Translate(Vector3.forward * vertical * m_SpeedMove * Time.deltaTime); 
        var movement = Vector3.forward * vertical * m_SpeedMove;
        m_Body.MovePosition(m_Body.position + movement);

        float speed = m_Body.velocity.magnitude;
        m_Animator.SetFloat("Speed", speed);
    }
}