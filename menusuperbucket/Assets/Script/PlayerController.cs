using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] AudioSource jumpSource;
    [SerializeField] AudioClip[] jumpSounds = new AudioClip[4];
    [SerializeField] AudioSource stepSource;
    [SerializeField] AudioClip[] stepSounds = new AudioClip[5];

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
    [SerializeField] AudioSource attackSource;
    [SerializeField] AudioClip[] attackSounds = new AudioClip[4];
    [SerializeField] AudioSource hitSource;
    [SerializeField] AudioClip[] enemyHitSounds = new AudioClip[10];

    [Header("Dash")]
    [SerializeField] ParticleSystem dashparticles;
    [SerializeField] AudioSource dashSound;

    [Header("Throw")]
    [SerializeField] bool canThrow = false;
    [SerializeField] float throwCooldown = 1.5f;
    [SerializeField] [Tooltip("Fuerza de lanzamiento.")] float throwSpeed = 10;
    [SerializeField] GameObject projectile;

    float throwTimer;

    [Header("Grapple")]
    public bool CanGrapple = false;
    [SerializeField] [Tooltip("M�xima distance de gancho.")] float maxGrappleDistance = 7;
    [SerializeField] [Tooltip("Velocidad transmitida al recoger el gancho.")] float grappleSpeed = 10;
    [SerializeField] float swingAcceleration = 3;
    [SerializeField] float maxSwingSpeed = 6;
    [SerializeField] float positionBias = 0.25f;
    [SerializeField] float gravityMultiplier = 2;
    [SerializeField] float speedBoost = 1.5f;
    [SerializeField] LayerMask grappleLayer;
    [SerializeField] LineRenderer grappleLine;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] AudioSource grappleSource;
    [SerializeField] AudioClip[] grappleSounds = new AudioClip[3];

    [Header("Platform")]
    public bool CanPlatform = false;
    [SerializeField] float platformCooldown = 3f;
    [SerializeField] GameObject platform;

    float platformTimer;

    #region General Use

    PlayerState _state;

    PlayerState State
    {
        get => _state;
        set
        {
            _state?.onEnd();
            _state = value;
        }
    }


    Rigidbody2D rb;
    Animator animator;

    //Script to handle all player movement.
	PlayerMovement movementHandler;

    bool grounded;
    public bool Stunned
    {
        get => State is StunState;
        set
        {
            if(value)
            {
                State = new StunState(this);
            }
            else
            {
                State = new NormalState(this);
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

        State = new NormalState(this);

        movementHandler.onAwake();
    }

    //Referencias a otros objetos
    void Start()
    {
        movementHandler.onStart();   
    }

    void Update()
    {
        timers();

        State.onUpdate();
    }

    private void FixedUpdate()
    {
        State.onFixedUpdate();
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
        animator.SetBool("Grounded", isGrounded());
    }

    bool isGrounded()
    {
        return movementHandler.IsGrounded();
    }

    void tryDash()
    {
        if(movementHandler.IsDashing)
        {
            State = new DashState(this);
        }
    }

    public void jumpSound()
    {
        jumpSource.PlayOneShot(jumpSounds[Random.Range(0, jumpSounds.Length)]);
    }

    public void stepSound()
    {
        stepSource.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);
    }
    #endregion

    #region Attack

    void tryAttack()
    {
        if (State is AttackState) return;

        if (InputManager.Attack && canAttack)
        {
            State = new AttackState(this, AttackState.AttackType.Weak);
        }

        if (InputManager.StrongAttack && canAttack)
        {
            State = new AttackState(this, AttackState.AttackType.Strong);
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
        if(platformTimer > 0)
        {
            platformTimer -= Time.deltaTime;
        }

        if(throwTimer > 0)
        {
            throwTimer -= Time.deltaTime;
        }
    }
    #endregion

    #region Aim
    void tryThrow()
    {
        if(InputManager.Throw && canThrow)
        {
            if(throwTimer <= 0)
            {
                Vector2 throwDirection = (direction * Vector2.right + Vector2.up).normalized;
                Instantiate(projectile, transform.position + new Vector3(throwDirection.x, throwDirection.y) * 1.5f, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = throwDirection * throwSpeed;
                throwTimer = throwCooldown;
            }
        }

    }

    void tryGrapple()
    {
        if(InputManager.Grapple && CanGrapple)
        {
            Vector2 grapplePosition = (Vector2) transform.position + rb.velocity * positionBias;
            Collider2D[] candidates = Physics2D.OverlapCircleAll(transform.position,  maxGrappleDistance, grappleLayer);
            if (candidates.Length != 0)
            {
                float dist = Mathf.Infinity, distpos = Mathf.Infinity;
                int index = -1, indexpos = -1;
                for(int i = 0; i < candidates.Length; i++)
                {
                    //We check the non obstructed grapple points with a bias in position given by the velocity
                    RaycastHit2D h = Physics2D.Raycast(grapplePosition, candidates[i].ClosestPoint(grapplePosition) - grapplePosition, maxGrappleDistance, groundLayer);
                    float disti = Vector2.Distance(grapplePosition, candidates[i].ClosestPoint(grapplePosition));
                    if (h.collider == candidates[i])
                    {
                        //Only take the grapples higher than the player
                        if (disti < dist && transform.position.y < candidates[i].ClosestPoint(grapplePosition).y)
                        {
                            index = i;
                            dist = disti;
                        }
                    }

                    // If none is non-obstructed, we just get the closest one
                    if (disti < distpos && transform.position.y < candidates[i].ClosestPoint(grapplePosition).y)
                    {
                        indexpos = i;
                        distpos = disti;
                    }
                }
                if(index != -1)
                {
                    State = new GrappleState(this, candidates[index].ClosestPoint(grapplePosition), grappleLine);
                }
                else if(indexpos != -1)
                {
                    State = new GrappleState(this, candidates[indexpos].ClosestPoint(grapplePosition), grappleLine);
                }
            }
        }
    }

    #endregion

    #region Platform
    void tryPlatform()
    {
        if(InputManager.Platform && CanPlatform)
        {
            createPlatform();
        }
    }

    void createPlatform()
    {
        if(platformTimer <= 0)
        {
            Instantiate(platform, transform.position + (GetComponent<BoxCollider2D>().size.y/2 + 0.2f) * Vector3.down, Quaternion.identity);
            platformTimer = platformCooldown;
        }
    }
    #endregion

    #region Player States

    abstract class PlayerState
    {
        public abstract void onUpdate();

        public virtual void onFixedUpdate()
        {
            return;
        }

        public virtual void onEnd()
        {
            return;
        }
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
            player.movementHandler.onUpdate();

            player.horizontalMovement();
            player.verticalMovement();

            //Astralis
            player.tryAttack();
            player.tryDash();

            //Prometeus
            player.tryThrow();
            player.tryGrapple();
            player.tryPlatform();
        }

        public override void onFixedUpdate()
        {
            player.movementHandler.onFixedUpdate();
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
            player.dashparticles.transform.rotation = Quaternion.Euler(0, 0, player.direction == 1 ? 0 : 180);
            player.dashparticles.Play();
            player.dashSound.Play();
        }

        public override void onUpdate()
        {
            if(!player.movementHandler.IsDashing)
            {
                player.State = new NormalState(player);
                player.animator.SetBool("Dashing", false);
            }
        }

        public override void onEnd()
        {
            player.animator.SetBool("Dashing", false);
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
                    player.attackSource.PlayOneShot(player.attackSounds[3]);
                    break;
                default:
                    break;
            }

        }

        public override void onUpdate()
        {
            player.movementHandler.onUpdate();

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
                player.State = new NormalState(player);
            }

        }

        public override void onFixedUpdate()
        {
            player.movementHandler.onFixedUpdate();
        }

        void weakAttack()
        {
            if (attackCount == 0 || (attackCount == 1 && attackTimer > player.secondAttackDelay))
            {

                hurtAll(Physics2D.OverlapBoxAll(player.attackHitbox.transform.position, player.attackHitbox.size, 0), player.weakDamage);


                if (attackCount == 0)
                {
                    player.animator.Play("Attack2", -1, 0);
                }
                else
                {
                    player.animator.Play("Attack1");
                }

                player.attackSource.PlayOneShot(player.attackSounds[Random.Range(0, player.attackSounds.Length - 1)]);
                attackTimer = 0;
                attackCount++;
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
                hurtAll(Physics2D.OverlapBoxAll(player.strongAttackHitbox.transform.position, player.strongAttackHitbox.size, 0), player.strongDamage);

                player.StartCoroutine(player.slideForward());
                attackCount++;
            }
        }

        void hurtAll(Collider2D[] hit, int damage)
        {
            bool enemyHit = false;
            foreach (Collider2D h in hit)
            {
                if (hurt(h, damage)) enemyHit = true;
            }

            if(enemyHit)
            {
                player.playRandomSound(player.hitSource, player.enemyHitSounds);
            }
        }

        bool hurt(Collider2D h, int damage)
        {
            if (h.transform == player.transform) return false;

            if (h.TryGetComponent(out IPushable pushable))
            {
                pushable.push(Mathf.Sign(h.transform.position.x - player.transform.position.x) * Vector2.right);
            }

            if (h.TryGetComponent(out HealthManager healthManager))
            {
                Vector3 spawnPos = h.ClosestPoint(player.transform.position);
                Instantiate(player.HitVFX, spawnPos, Quaternion.LookRotation(player.transform.position - spawnPos));
                healthManager.Health -= damage;
                return true;
            }
            return false;
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
            player.movementHandler.onUpdate();
        }

        public override void onFixedUpdate()
        {
            player.movementHandler.onFixedUpdate();
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

            grappleLine.SetPosition(0, player.transform.position);
            grappleLine.SetPosition(1, new Vector3(target.x, target.y));

            distance = Vector3.Distance(target, (Vector2)player.transform.position);
            initialDistance = distance;

            player.rb.velocity = (target - (Vector2)player.transform.position).normalized * player.grappleSpeed;

            player.grappleSource.PlayOneShot(player.grappleSounds[0]);
        }

        public override void onUpdate()
        {
            if(InputManager.Grapple)
            {
                recogiendoCable = true;
                player.rb.velocity = (target - (Vector2)player.transform.position).normalized * player.grappleSpeed;
                player.grappleSource.PlayOneShot(player.grappleSounds[1]);
            }
            else if(InputManager.JumpPressed)
            {
                player.State = new NormalState(player);
                player.rb.velocity *= player.speedBoost;
                player.rb.gravityScale = player.gravityMultiplier;
                grappleLine.gameObject.SetActive(false);
                player.grappleSource.PlayOneShot(player.grappleSounds[2]);
            }

            if (firstApproach)
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
                    player.State = new NormalState(player);
                    player.rb.gravityScale = player.movementHandler.gravityScale;
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


                if (Vector3.Distance(player.transform.position, target) >= distance)
                {
                    Vector2 direction = ((Vector2)player.transform.position - target).normalized;
                    Vector2 ort = new Vector2(direction.y, -direction.x);

                    if(Vector2.Dot(player.hInput * ort, Vector2.up) < 0.5)
                    {
                        player.rb.velocity += Vector2.Dot(ort, Vector2.right) * player.hInput * ort * player.swingAcceleration * Time.deltaTime;
                        if(player.rb.velocity.magnitude > player.maxSwingSpeed)
                            player.rb.velocity = player.rb.velocity.normalized * player.maxSwingSpeed;
                    }
                    //Adjust position to swing

                    player.transform.position = target + distance * direction;
                    player.rb.velocity = Vector2.Dot(player.rb.velocity, ort) * ort;
                }
            }

            grappleLine.SetPosition(0, player.transform.position);
            grappleLine.SetPosition(1, new Vector3(target.x, target.y));
            
        }

        public override void onEnd()
        {
            player.rb.gravityScale = player.movementHandler.gravityScale;
            grappleLine.gameObject.SetActive(false);
        }
    }
    #endregion

    #endregion

    void playRandomSound(AudioSource source, AudioClip[] clips)
    {
        source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}
