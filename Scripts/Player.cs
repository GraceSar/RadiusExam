using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public static Player Singleton { get; private set; }

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Die = Animator.StringToHash("Die");

    [SerializeField] private LayerMask m_Enemy;
    [SerializeField] private LayerMask m_Ground;

    // Component
    private Animator m_Anim;
    private NavMeshAgent m_Agent;

    // Enemy
    private Transform m_Target;

    [SerializeField] private float attackRange = 2f;

    Transform cachedTransform;


    const float MAX_HP = 100;
    float hp = MAX_HP;

    [SerializeField] private float atk = 10;

    bool isAttacking;

    private void Awake()
    {
        if (Singleton == null) Singleton = this;
    }

    private void OnDestroy()
    {
        if (Singleton == this) Singleton = null;
    }

    void Start()
    {
        m_Anim = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();

        cachedTransform = transform;
    }

    // Update is called once per frame
    private void Update()
    {
        // Left Click on Enemy
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, m_Enemy))
            {
                m_Target = hit.collider.transform;
                m_Anim.Play(Walk);

                m_Agent.SetDestination(m_Target.position);
            }
        }

        // Right Click on Ground
        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_Ground))
            {
                m_Target = null;
                m_Anim.Play(Walk);

                m_Agent.SetDestination(hit.point);
            }
        }

        if (m_Target != null)
        {
            if (IsInAttackRange())
            {
                m_Agent.Stop();

                m_Anim.Play(Attack);

                isAttacking = true;
            }
            else
            {
                isAttacking = false;
            }

            if (isAttacking)
            {
                m_Target.GetComponent<Enemy>().TakeDamage(Mathf.FloorToInt(atk * Time.deltaTime));
            }
        }

    }

    private bool IsInAttackRange()
    {
        if (m_Target != null)
        {
            return (cachedTransform.position - m_Target.position).sqrMagnitude <= (attackRange * attackRange);
        }
        return false;
    }
}