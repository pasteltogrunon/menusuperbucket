using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Horizontal")]
    [SerializeField] [Tooltip("Velocidad máxima.")] float maxSpeed = 10;
    [SerializeField] [Tooltip("Aceleración del movimiento horizontal.")] float acceleration = 10;
    [SerializeField] [Tooltip("Lo que tarda en frenarse si no se pulsa ninguna tecla (cuanto más, menos tarda).")] float groundFriction = 0.2f;
    [SerializeField] [Tooltip("Reducción de movilidad en el aire (0 es nada, 1 es la misma que en el suelo).")] float onAirReduction = 0.3f;

    [Header("Verical")]
    [SerializeField] [Tooltip("Fuerza de salto, cuanto más, más alto salta.")] float jumpForce = 10;
    [SerializeField] [Tooltip("Multiplicador de gravedad cuando no se pulsa el espacio.")] float gravityMultiplier = 3;
    [SerializeField] [Tooltip("Multiplicador de gravedad cuando va hacia abajo.")] float gravityMultiplierDown = 4;
    [SerializeField] float secondJumpForce = 10;
    [SerializeField] LayerMask groundLayer;
    public bool canDoubleJump;

    int jumpCount = 0;

    [Header("Dash")]
    [SerializeField] bool canDash;
    [SerializeField] float dashTime = 0.5f;
    [SerializeField] float dashSpeed = 20;
    [SerializeField] float residualSpeed = 4;
    [SerializeField] float dashCooldown = 0.5f;

    float dashTimer = 0;

    [Header("Attack")]
    [SerializeField] bool canAttack = false;
    [SerializeField] int damage = 5;
    [SerializeField] float attackMargin = 0.4f;
    [SerializeField] float secondAttackDelay = 0.1f;
    [SerializeField] BoxCollider2D attackHitbox;

    int attackCount = 0;
    float attackTimer = 0;


    [Header("Throw")]
    [SerializeField] bool canThrow = false;
    [SerializeField] float throwSpeed = 10;
    [SerializeField] GameObject projectile;

    PlayerState state;


    Rigidbody2D rb;
    Animator animator;
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

    #region Movement
    void horizontalMovement()
    {
        if(grounded)
        {
            //If the player is grounded we check the movement normally
            if (hInput != 0)
            {
                //Not adding to the speed if over the limits
                if(!(hInput == 1 && speed >= maxSpeed) && !((hInput == -1 && speed <= -maxSpeed)))
                {
                    speed += hInput * acceleration * Time.deltaTime;
                }
            }
            else
            {
                //If not pressing any key, decelerate the player
                speed = speed * Mathf.Pow(groundFriction, 1 / Time.deltaTime);
            }
        }
        else
        {
            //Reduce the movemnt capacity in the air
            if (hInput != 0)
            {
                if (!(hInput == 1 && speed >= maxSpeed *onAirReduction) && !((hInput == -1 && speed <= -maxSpeed * onAirReduction)))
                    speed += hInput * acceleration * Time.deltaTime ;
            }
        }

        if(hInput != 0)
        {
            direction = Mathf.Sign(hInput);
        }

        animator.SetFloat("Speed", Mathf.Abs(speed));
    }

    void verticalMovement()
    {

        if(InputManager.JumpPressed)
        {
            if(grounded)
            {
                rb.velocity = speed * Vector2.right + jumpForce * Vector2.up;
            }
            else if(jumpCount == 1)
            {
                rb.velocity = speed * Vector2.right + secondJumpForce * Vector2.up;
                jumpCount--;
            }
        }


        //If space if kept pressed, the gravity is smaller, so the jump goes higher (only when going up)
        if(InputManager.JumpHolded && rb.velocity.y >= 0)
        {
            rb.gravityScale = 1;
        }
        else
        {
            if(!grounded)
                rb.velocity -= (rb.velocity.y >= 0 ? gravityMultiplier : gravityMultiplierDown) * Time.deltaTime * Vector2.up;
        }

        animator.SetFloat("Vertical Speed", rb.velocity.y);
    }

    bool isGrounded()
    {
        //Casting a ray downwards
        if (Physics2D.Raycast(transform.position, Vector2.down, GetComponent<BoxCollider2D>().size.y / 2 + 0.2f, groundLayer))
        {
            if(canDoubleJump)
                jumpCount = 1;
            return true;
        }
        return false;
    }

    void tryDash()
    {
        if(InputManager.Dash && canDash && dashTimer == 0)
        {
            state = new DashState(this);
        }
    }

    #endregion

    #region Attack
    void tryAttack()
    {
        if (InputManager.Attack && canAttack)
        {
            attack();
        }
    }

    void attack()
    {
        if (attackCount == 0 || attackCount == 1 && attackTimer > secondAttackDelay)
        {
            Collider2D[] hit = Physics2D.OverlapBoxAll(attackHitbox.transform.position, attackHitbox.size, 0);

            foreach (Collider2D h in hit)
            {
                hurt(h);
            }

            if (attackCount == 0)
            {
                animator.Play("Astralis_Attack1", -1, 0);
            }
            else
            {
                animator.Play("Astralis_Attack2");
            }

            attackTimer = 0;
            attackCount++;

            StartCoroutine(slideForward());
        }

    }

    void hurt(Collider2D h)
    {
        if (h.TryGetComponent(out HealthManager healthManager))
        {
            healthManager.Health -= damage;
        }

        if (h.TryGetComponent(out IPushable pushable))
        {
            pushable.push(Mathf.Sign(h.transform.position.x - transform.position.x) * Vector2.right);
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
        if(dashTimer != 0)
            dashTimer = Mathf.Max(0, dashTimer - Time.deltaTime);

        if (attackCount > 0)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > attackMargin)
            {
                attackTimer = 0;
                attackCount = 0;
            }
        }
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
            Vector2 throwDirection = aimDirection;
            Instantiate(projectile, transform.position + new Vector3(throwDirection.x, throwDirection.y) * 1.5f, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = throwDirection * throwSpeed;
        }

    }

    #endregion

    #region Player States
    abstract class PlayerState
    {
        public abstract void onUpdate();
    }

    class NormalState : PlayerState
    {
        PlayerController player;

        public NormalState(PlayerController player)
        {
            this.player = player;
        }

        public override void onUpdate()
        {
            player.grounded = player.isGrounded();

            player.horizontalMovement();

            player.verticalMovement();

            //Astralis
            player.tryAttack();
            player.tryDash();

            //Prometeus
            player.tryThrow();
        }
    }

    class DashState : PlayerState
    {
        PlayerController player;
        float dashTime = 0;

        public DashState(PlayerController player)
        {
            this.player = player;
            dashTime = player.dashTime;

            player.rb.velocity = player.dashSpeed * player.transform.localScale.x * Vector2.right;
            player.rb.gravityScale = 0;
        }

        public override void onUpdate()
        {
            dashTime -= Time.deltaTime;
            if(dashTime < 0)
            {
                player.state = new NormalState(player);
                player.speed = player.residualSpeed;
                player.dashTimer = player.dashCooldown;
                player.rb.gravityScale = 1;
            }
        }
    }

    class StunState : PlayerState
    {
        PlayerController player;

        public StunState(PlayerController player)
        {
            this.player = player;
        }

        public override void onUpdate()
        {
            player.grounded = player.isGrounded();

            if(!player.grounded)
                player.rb.gravityScale = player.rb.velocity.y >= 0 ? player.gravityMultiplier : player.gravityMultiplierDown;
        }
    }
    #endregion
}
