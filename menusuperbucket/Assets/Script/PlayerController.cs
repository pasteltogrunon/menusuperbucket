using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Stun Gravity")]
    [SerializeField] [Tooltip("Multiplicador de gravedad cuando no se pulsa el espacio.")] float gravityMultiplier = 3;
    [SerializeField] [Tooltip("Multiplicador de gravedad cuando va hacia abajo.")] float gravityMultiplierDown = 4;

    [Header("Attack")]
    [SerializeField] bool canAttack = false;
    [SerializeField] [Tooltip("Danio del ataque debil.")] int weakDamage = 5;
    [SerializeField] [Tooltip("Danio del ataque fuerte.")] int strongDamage = 10;
    [SerializeField] [Tooltip("Margen para volver a atacar (segundo ataque).")] float attackMargin = 0.4f;
    [SerializeField] [Tooltip("Tiempo minimo para el segundo ataque.")] float secondAttackDelay = 0.1f;
    [SerializeField] [Tooltip("Tiempo de retardo para el ataque fuerte.")] float strongAttackDelay = 0.1f;
    [SerializeField] [Tooltip("Frenado cuando se usa el ataque fuerte. 1 es nada, 0 es frenada completa.")] float strongAttackDecelaration = 0.3f;
    [SerializeField] BoxCollider2D attackHitbox;
    [SerializeField] BoxCollider2D strongAttackHitbox;
    [SerializeField] GameObject HitVFX;

    int attackCount = 0;
    float attackTimer = 0;

    [Header("Throw")]
    [SerializeField] bool canThrow = false;
    [SerializeField] [Tooltip("Fuerza de lanzamiento.")] float throwSpeed = 10;
    [SerializeField] GameObject projectile;

    [Header("Grapple")]
    [SerializeField] bool canGrapple = false;
    [SerializeField] [Tooltip("M�xima distance de gancho.")] float maxGrappleDistance = 7;
    [SerializeField] [Tooltip("Velocidad transmitida al recoger el gancho.")] float grappleSpeed = 10;
    [SerializeField] LayerMask grappleLayer;
    [SerializeField] LineRenderer grappleLine;

    #region General Use

    PlayerState state;

    Rigidbody2D rb;
    Animator animator;

    //Script to handle all player movement.
	PlayerMovement movementHandler;

    bool grounded;
    public bool Stunned
    {
        get => state is StunState;
        set
        {
            if(value)
            {
                state = new StunState(this);
            }
            else
            {
                state = new NormalState(this);
            }
        }
    }

    float speed
    {
        get => rb.velocity.x;
        set
        {
            rb.velocity = value * Vector2.right + rb.velocity.y * Vector2.up;
        }
    }

    float direction
    {
        get => Mathf.Sign(transform.localScale.x);
        set
        {
            transform.localScale = new Vector3(value, 1, 1);
        }
    }

    float hInput
    {
        get => (InputManager.Left ? -1 : 0) + (InputManager.Right ? 1 : 0);
    }

    //Referencias a uno mismo
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
		movementHandler = GetComponent<PlayerMovement>();

        state = new NormalState(this);
    }

    //Referencias a otros objetos
    void Start()
    {
        
    }

    void Update()
    {
        timers();

        state.onUpdate();
    }
    #endregion

    #region Movement

    void horizontalMovement()
    {
        if(hInput != 0)
            direction = Mathf.Sign(hInput);
        
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void verticalMovement()
    {
        animator.SetFloat("Vertical Speed", rb.velocity.y);
    }

    bool isGrounded()
    {
        return movementHandler.IsGrounded();
    }

    void tryDash()
    {
        if(movementHandler.IsDashing)
        {
            state = new DashState(this);
        }
    }
    #endregion

    #region Attack

    void tryAttack()
    {
        if (state is AttackState) return;

        if (InputManager.Attack && canAttack)
        {
            state = new AttackState(this, AttackState.AttackType.Weak);
        }

        if (InputManager.StrongAttack && canAttack)
        {
            state = new AttackState(this, AttackState.AttackType.Strong);
        }
    }


    IEnumerator slideForward()
    {
        float oldSpeed = speed;
        speed = 10 * direction;
        yield return new WaitForSeconds(0.05f);
        speed = oldSpeed;
    }

    void timers()
    {

    }
    #endregion

    #region Aim
    Vector2 aimDirection
    {
        get => InputManager.getAimDirection(transform.position);
    }

    void tryThrow()
    {
        if(InputManager.Throw && canThrow)
        {
            Vector2 throwDirection = (direction * Vector2.right + Vector2.up).normalized;
            Instantiate(projectile, transform.position + new Vector3(throwDirection.x, throwDirection.y) * 1.5f, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = throwDirection * throwSpeed;
        }

    }

    void tryGrapple()
    {
        if(InputManager.Grapple && canGrapple)
        {
            Vector2 grappleDirection = aimDirection;
            Collider2D hit = Physics2D.OverlapCircle(transform.position,  maxGrappleDistance, grappleLayer);
            if (hit)
            {
                state = new GrappleState(this, hit.transform.position, grappleLine);
            }
        }
    }

    #endregion

    #region Player States

    abstract class PlayerState
    {
        public abstract void onUpdate();
    }

    #region Normal State

    class NormalState : PlayerState
    {
        PlayerController player;

        public NormalState(PlayerController player)
        {
            this.player = player;

            player.animator.SetBool("Dashing", false);
        }

        public override void onUpdate()
        {
            player.horizontalMovement();
            player.verticalMovement();

            //Astralis
            player.tryAttack();
            player.tryDash();

            //Prometeus
            player.tryThrow();
            player.tryGrapple();
        }
    }
    #endregion

    #region Dash State

    class DashState : PlayerState
    {
        PlayerController player;
        float dashTime = 0;

        public DashState(PlayerController player)
        {
            this.player = player;
            player.animator.SetBool("Dashing", true);
        }

        public override void onUpdate()
        {
            if(!player.movementHandler.IsDashing)
            {
                player.state = new NormalState(player);
                player.animator.SetBool("Dashing", false);
            }
        }
    }
    #endregion

    #region Attack State

    class AttackState : PlayerState
    {
        PlayerController player;
        AttackType attackType;
        int attackCount = 0;
        float attackTimer = 0;

        public AttackState(PlayerController player, AttackType attackType)
        {
            this.player = player;
            this.attackType = attackType;

            switch (attackType)
            {
                case AttackType.Weak:
                    weakAttack();
                    break;
                case AttackType.Strong:
                    player.animator.Play("Strong", -1, 0);
                    player.speed *= player.strongAttackDecelaration;
                    break;
                default:
                    break;
            }

            player.animator.SetBool("Dashing", false);
        }

        public override void onUpdate()
        {
            switch(attackType)
            {
                case AttackType.Weak:
                    player.horizontalMovement();
                    player.verticalMovement();

                    trySecondAttack();
                    break;
                case AttackType.Strong:
                    if (attackTimer > player.strongAttackDelay)
                        strongAttack();
                    break;
                default:
                    break;
            }

            attackTimer += Time.deltaTime;
            if (attackTimer > player.attackMargin)
            {
                player.state = new NormalState(player);
            }
        }

        void weakAttack()
        {
            if (attackCount == 0 || attackCount == 1 && attackTimer > player.secondAttackDelay)
            {
                Collider2D[] hit = Physics2D.OverlapBoxAll(player.attackHitbox.transform.position, player.attackHitbox.size, 0);

                foreach (Collider2D h in hit)
                {
                    hurt(h, player.weakDamage);
                }

                if (attackCount == 0)
                {
                    player.animator.Play("Attack2", -1, 0);
                }
                else
                {
                    player.animator.Play("Attack1");
                }

                attackTimer = 0;
                attackCount++;

                player.StartCoroutine(player.slideForward());
            }

        }

        void trySecondAttack()
        {
            if (InputManager.Attack)
            {
                weakAttack();
            }
        }

        void strongAttack()
        {
            if(attackCount == 0)
            {
                Collider2D[] hit = Physics2D.OverlapBoxAll(player.strongAttackHitbox.transform.position, player.strongAttackHitbox.size, 0);

                foreach (Collider2D h in hit)
                {
                    hurt(h, player.strongDamage);
                }

                player.StartCoroutine(player.slideForward());
                attackCount++;
            }
        }

        void hurt(Collider2D h, int damage)
        {
            if (h.transform == player.transform) return;

            if (h.TryGetComponent(out HealthManager healthManager))
            {
                Instantiate(player.HitVFX, h.ClosestPoint(player.transform.position), Quaternion.identity);
                healthManager.Health -= damage;
            }

            if (h.TryGetComponent(out IPushable pushable))
            {
                pushable.push(Mathf.Sign(h.transform.position.x - player.transform.position.x) * Vector2.right);
            }
        }

        public enum AttackType
        {
            Weak,
            Strong,
            Charged
        }
    }
    #endregion

    #region Stun State

    class StunState : PlayerState
    {
        PlayerController player;

        public StunState(PlayerController player)
        {
            this.player = player;

            player.animator.SetBool("Dashing", false);
        }

        public override void onUpdate()
        {
            if(!player.isGrounded())
                player.rb.gravityScale = player.rb.velocity.y >= 0 ? player.gravityMultiplier : player.gravityMultiplierDown;
        }
    }
    #endregion

    #region Grapple State
    
    class GrappleState : PlayerState
    {
        PlayerController player;
        LineRenderer grappleLine;

        Vector2 target;
        float distance = 0;
        float initialDistance = 0;

        bool recogiendoCable = false;
        bool firstApproach = true;

        public GrappleState(PlayerController player, Vector2 target, LineRenderer grappleLine)
        {
            this.player = player;
            this.target = target;
            this.grappleLine = grappleLine;

            grappleLine.gameObject.SetActive(true);

            distance = Vector3.Distance(target, (Vector2)player.transform.position);
            initialDistance = distance;

            player.rb.velocity = (target - (Vector2)player.transform.position).normalized * player.grappleSpeed;

            player.animator.SetBool("Dashing", false);
        }

        public override void onUpdate()
        {
            if(InputManager.Grapple)
            {
                recogiendoCable = true;
                player.rb.velocity = (target - (Vector2)player.transform.position).normalized * player.grappleSpeed;
            }
            else if(InputManager.JumpPressed)
            {
                player.state = new NormalState(player);
                //player.dashTimer = player.dashCooldown;
                player.rb.gravityScale = 1;
                grappleLine.gameObject.SetActive(false);
            }

            if(firstApproach)
            {
                //Nada m�s empezar se acerca un poco
                distance -= Time.deltaTime * 10;
                player.rb.gravityScale = 0;
                if (distance <= 0.75f * initialDistance)
                {
                    firstApproach = false;
                    player.rb.gravityScale = 1;
                    player.rb.velocity = Vector2.zero;
                }
            }
            else if(recogiendoCable)
            {
                distance -= Time.deltaTime * 10;
                player.rb.gravityScale = 0;
                if (distance <= 0)
                {
                    player.state = new NormalState(player);
                    //player.dashTimer = player.dashCooldown;
                    player.rb.gravityScale = 1;
                    grappleLine.gameObject.SetActive(false);
                }
            }
            else
            {
                //Pretty much the normal state without the grapple
                player.horizontalMovement();
                player.verticalMovement();

                //Astralis
                player.tryAttack();
                player.tryDash();

                //Prometeus
                player.tryThrow();

                if (Vector3.Distance(player.transform.position, target) > distance)
                {
                    //Adjust position to swing
                    Vector2 direction = ((Vector2)player.transform.position - target).normalized;
                    Vector2 ort = new Vector2(direction.y, -direction.x);

                    player.transform.position = target + distance * direction;
                    player.rb.velocity = Vector2.Dot(player.rb.velocity, ort) * ort;
                }
            }

            grappleLine.SetPosition(0, player.transform.position);
            grappleLine.SetPosition(1, new Vector3(target.x, target.y));
            
        }
    }
    #endregion

    #endregion
}
