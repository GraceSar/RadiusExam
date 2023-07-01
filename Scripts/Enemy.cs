using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Die = Animator.StringToHash("Die");

    // Component
    private Animator m_Anim;
    private NavMeshAgent m_Agent;

    // Enemy
    private Transform m_Target;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float maxFollowRange = 10f;

    const float MAX_HP = 100;
    float hp = MAX_HP;

    Transform cachedTransform;

    [SerializeField] private LayerMask m_Player;

    [SerializeField]
    private Transform baseTransform;

    enum State
    {
        Idle,
        Active,
        Dead,
    }
    State state;

    void Start()
    {
        m_Anim = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();
        cachedTransform = transform;
    }

    private void Update()
    {
        switch(state)
        {
            case State.Idle:
                if (Physics.CheckSphere(cachedTransform.position, attackRange, m_Player))
                {
                    m_Target = Player.Singleton.transform;
                    state = State.Active;
                }

                break;

            case State.Active:
                if (IsInAttachRange())
                {
                    // Attack
                    m_Anim.Play(Attack);
                }
                else
                {
                    m_Anim.Play(Walk);

                    if (IsOutOfAttackRange())
                    {
                        // Walk back
                        m_Agent.SetDestination(baseTransform.position);

                        if ((cachedTransform.position - baseTransform.position).sqrMagnitude < 0.01f)
                        {
                            m_Agent.Stop();
                            state = State.Idle;
                        }
                    }
                    else
                    {
                        // Follow
                        m_Agent.SetDestination(m_Target.position);
                    }
                }
                break;

            case State.Dead:
                break;
        }

        //if (m_Target == null)
        //{
        //    if (Physics.CheckSphere(cachedTransform.position, attackRange, m_Player))
        //    {
        //        m_Target = Player.Singleton.transform;
        //    }
        //}
        //else
        //{
        //    if (IsInAttachRange())
        //    {
        //        // Attack
        //        m_Anim.Play(Attack);
        //    }
        //    else
        //    {
        //        m_Anim.Play(Walk);

        //        if (IsOutOfAttackRange())
        //        {
        //            // Walk back
        //            m_Agent.SetDestination(baseTransform.position);
        //        }
        //        else
        //        {
        //            // Follow
        //            m_Agent.SetDestination(m_Target.position);
        //        }
        //    }
        //}
    }

    bool IsInAttachRange()
    {
        if (m_Target != null)
        {
            return (cachedTransform.position - m_Target.position).sqrMagnitude <= (attackRange * attackRange);
        }
        return false;
    }

    bool IsOutOfAttackRange()
    {
        if (m_Target != null)
        {
            return (cachedTransform.position - m_Target.position).sqrMagnitude <= (maxFollowRange * maxFollowRange);
        }
        return false;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
    }

    private void OnGUI()
    {
        GUILayout.Label("IsInAttachRange(): " + IsInAttachRange());
        GUILayout.Label("m_Target " + (m_Target ? m_Target.position.ToString() : "null"));
        GUILayout.Label("HP: " + hp);
    }
}