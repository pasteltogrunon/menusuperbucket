using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Movement Data

    [Header("Walk / Run")]

	[SerializeField] [Tooltip("Velocidad máxima.")] float runMaxSpeed;
	[SerializeField] [Tooltip("Aceleración del movimiento horizontal.")] float runAcceleration;
	[HideInInspector] public float runAccelAmount; //Fuerza aplicada al jugador, multiplicada por speedDiff.
	[SerializeField] [Tooltip("Lo que tarda en frenarse si no se pulsa ninguna tecla (cuanto más, menos tarda).")] float groundFriction;
	[HideInInspector] public float runDeccelAmount; //Fuerza aplicada al jugador, multiplicada por speedDiff.
	[Space(5)]
	[Range(0f, 1)] [SerializeField] [Tooltip("Reducción de movilidad en el aire (0 es nada, 1 es la misma que en el suelo).")] float onAirReduction;
	[Space(5)]
	[SerializeField] [Tooltip("Pues eso, que si conserva el momento (duuhh).")] bool doConserveMomentum = true;

	[Space(5)]
    
    [Header("Grace Periods")]

	[Range(0.01f, 0.5f)] [SerializeField] [Tooltip("Tiempo que tienes después de caer de una plataforma para poder saltar de todas maneras (esto se incluye por ti, auténtico manco).")] float coyoteTime;
	[Range(0.01f, 0.5f)] [SerializeField] [Tooltip("Tiempo que almacena un input de salto no exitoso para realizarlo automáticamente si se cumplen las condiciones (tocar hierba, que te va tocando).")] float jumpInputBufferTime;

	[Space(5)]

    [Header("Dash")]

	[SerializeField] [Tooltip("Cuántos dashes tienes mákena.")] int dashAmount;
	[SerializeField] [Tooltip("Cuánta velocidad alcanza el dash.")] float dashSpeed;
	[SerializeField] [Tooltip("Tiempo que se congela el juego al pulsar el dash y antes de leer el input direccional y aplicar la fuerza.")] float dashSleepTime;
	[Space(5)]
	[SerializeField] [Tooltip("Tiempo que realiza el dash (sí, llevamos dos parámetros sin comentario gracios, qué pasa).")] float dashAttackTime;
	[Space(5)]
	[SerializeField] [Tooltip("Tiempo. Final. Dash. In that order.")] float dashEndTime;
	[SerializeField] [Tooltip("Frena al jugador para hacer el dash más responsive.")] Vector2 dashEndSpeed;
	[Range(0f, 1f)] [SerializeField] [Tooltip("Disminuye la influencia del movimiento del jugador mientras dasheeaa, jalaaa, cabrón ya no te quedan baalaa'.")] float dashEndRunLerp; //Slows the affect of player movement while dashing
	[Space(5)]
	[SerializeField] [Tooltip("Cuánta tardas en poder volver a ejecutar un dash.")] float dashRefillTime;
	[Space(5)]
	[Range(0.01f, 0.5f)] [SerializeField] [Tooltip("No sabía si poner esto en grace periods o el apartado del dash, pero yatusabe.")] float dashInputBufferTime;

	//Gravity hiding
	[HideInInspector] public float gravityStrength; //La fuelsa de la graveda..
	[HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
										        //Also the value the player's rigidbody2D.gravityScale is set to. Esto se queda tal cual, cabron.
	
	[Space(5)]

    [Header("Gravity")]

	[SerializeField] [Tooltip("Multiplicador de la gravityScale del jugador cuando cae (tevacae (tevacae tu)).")] float fallGravityMult;
	[SerializeField] [Tooltip("Velocidad de caída máxima.")] float maxFallSpeed;
	[Space(5)]
	[SerializeField] [Tooltip("Aumento de velocidad de caída al usar un input hacia abajo.")] float fastFallGravityMult;
	[SerializeField] [Tooltip("Velocidad de caída rápida (por el input hacia abajo) máxima.")] float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.
	
	[Space(5)]

	[Header("Jump")]

	[SerializeField] [Tooltip("Altura del salto.")] float jumpHeight;
	[SerializeField] [Tooltip("Tiempo desde que se aplica la fuerza del salto hasta que se alcanza la altura deseada.")] float jumpTimeToApex;
	[HideInInspector] public float jumpForce; //Fuerza vertical aplicada al jugador.

	[Header("Both Jumps")]
	[SerializeField] [Tooltip("Multiplicador que aumenta la gravedad si el jugador suelta el botón de salto durante el propio salto.")] float jumpCutGravityMult;
	[Range(0f, 1)] [SerializeField] [Tooltip("Reduce la gravedad cerca de la cúspide del salto.")] float jumpHangGravityMult;
	[SerializeField] [Tooltip("Velocidades (cercanas a 0) a las que el jugador experimentará una reducción de gravedad (cerca de la cúspide).")] float jumpHangTimeThreshold;
	[SerializeField] [Tooltip("Yeye.")] float jumpHangAccelerationMult; 
	[SerializeField] [Tooltip("Dasrait.")] float jumpHangMaxSpeedMult;
	
    [Space(5)]

	[Header("Wall Jump")]
	[SerializeField] [Tooltip("Murosaltar o no Murosaltar, ésa es la cuestión.")] bool canWallJump = true;
    [SerializeField] [Tooltip("Fuerza aplicada al jugador cuando murosalta.")] Vector2 wallJumpForce;
	[Space(5)]
	[Range(0f, 1f)] [SerializeField] [Tooltip("Disminuye la influencia del movimiento del jugador mientras murosalteeaa, jalaaa, cabrón ya no te quedan baalaa'. Jajaja. Qué solo estoy, jo.")] float wallJumpRunLerp;
	[Range(0f, 1.5f)] [SerializeField] [Tooltip("Sssssiuuuuuu.")] float wallJumpTime;
	[SerializeField] [Tooltip("El jugador rota????? Hacia el muro murosaltado    ??????.")] bool doTurnOnWallJump;

	[Space(5)]

	[Header("Wall Slide")]
	[SerializeField] [Tooltip("Velocidad del murodeslizamiento.")] float slideSpeed;
	[SerializeField] [Tooltip("Aceleración del murodeslizamiento.")] float slideAccel;
	

	//Unity Callback, called when the inspector updates
    private void OnValidate()
    {
		//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
		
		//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
		gravityScale = gravityStrength / Physics2D.gravity.y;

		//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * groundFriction) / runMaxSpeed;

		//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
		groundFriction = Mathf.Clamp(groundFriction, 0.01f, runMaxSpeed);
	}

	#endregion

    #region Components

    public Rigidbody2D RB { get; private set; }
    PlayerController playerController;

    #endregion

    #region State Parameters

    //Variables para controlar los estados del jugador.
	public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; }
	public bool IsSliding { get; private set; }

	//Timers
	public float LastOnGroundTime { get; private set; }
	public float LastOnWallTime { get; private set; }
	public float LastOnWallRightTime { get; private set; }
	public float LastOnWallLeftTime { get; private set; }

	//Jump
	private bool _isJumpCut;
	private bool _isJumpFalling;

	//Wall Jump
	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	//Dash
	private int _dashesLeft;
	private bool _dashRefilling;
	private Vector2 _lastDashDir;
	private bool _isDashAttacking;

    #endregion

    #region Input Parameters

    private Vector2 _moveInput;

	public float LastPressedJumpTime { get; private set; }
	public float LastPressedDashTime { get; private set; }

    #endregion

    #region Check Parameters

	[Space(5)]

	[Header("Checks")]

	[SerializeField] [Tooltip("Punto de referencia de las comprobaciones con el suelo.")] private Transform _groundCheckPoint;
	[SerializeField] [Tooltip("Tamaño del suelocheckeo.")] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[Space(5)]
	[SerializeField] [Tooltip("Punto de referencia de las comprobaciones con la pared frontal.")] private Transform _frontWallCheckPoint;
	[SerializeField] [Tooltip("Punto de referencia de las comprobaciones con la pared trasera.")] private Transform _backWallCheckPoint;
	[SerializeField] [Tooltip("Tamaño de los murocheckeos.")] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

    #endregion

    #region Layers / Tags

	[Space(5)]

    [Header("Layers / Tags")]
	[SerializeField] [Tooltip("Estrato del suelocheckeo.")] private LayerMask _groundLayer;

	#endregion

    public void onAwake()
	{
		RB = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
	}

    #region General Use

    public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}

    //Method used so we don't need to call StartCoroutine everywhere.
	private void Sleep(float duration)
    {
		StartCoroutine(nameof(PerformSleep), duration);
    }

	private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration);
		Time.timeScale = 1;
	}

	public bool IsGrounded()
	{
		return Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);
	}

    #endregion

	public void onStart()
	{
		SetGravityScale(gravityScale);
		IsFacingRight = true;
	}

    public void onUpdate()
	{
        #region Update: Timers

        LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;
		LastOnWallRightTime -= Time.deltaTime;
		LastOnWallLeftTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		LastPressedDashTime -= Time.deltaTime;

		#endregion

		#region Update: Inputs

		_moveInput.x = InputManager.HorizontalAxis;

        _moveInput.y = InputManager.VerticalAxis;

		if (_moveInput.x != 0)
			IsFacingRight = (_moveInput.x > 0);

		if(InputManager.JumpPressed)
        {
			OnJumpInput();
        }

		if (InputManager.JumpUp)
		{
			OnJumpUpInput();
		}

		if (InputManager.Dash)
		{
			OnDashInput();
		}

		#endregion

		#region Update: Collisions

		if (!IsDashing && !IsJumping)
		{
			//Ground Check
			if (IsGrounded()) //Checks if set box overlaps with ground
			{
				/*if(LastOnGroundTime < -0.1f)
                {
					AnimHandler.justLanded = true;
                }*/

				LastOnGroundTime = coyoteTime; //If so sets the lastGrounded to coyoteTime
            }		

			//Right Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
					|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
				LastOnWallRightTime = coyoteTime;

			//Left Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
				LastOnWallLeftTime = coyoteTime;

			//Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
			LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
		}
		
		#endregion

		#region Update: Jump

		if (IsJumping && RB.velocity.y < 0)
		{
			IsJumping = false;

			_isJumpFalling = true;
		}

		if (IsWallJumping && Time.time - _wallJumpStartTime > wallJumpTime)
		{
			IsWallJumping = false;
		}

		if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
			_isJumpCut = false;

			_isJumpFalling = false;
		}

		if (!IsDashing)
		{
			//Jump
			if (CanJump() && LastPressedJumpTime > 0)
			{
				IsJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();

				//AnimHandler.startedJumping = true;
			}

			//Wall Jump
			else if (CanWallJump() && LastPressedJumpTime > 0)
			{
				IsWallJumping = true;
				IsJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;

				_wallJumpStartTime = Time.time;
				_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

				WallJump(_lastWallJumpDir);
			}
		}

		#endregion

		#region Update: Dash

		if (CanDash() && LastPressedDashTime > 0)
		{
			//Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
			Sleep(dashSleepTime); 

			//If not direction pressed, dash forward
			if (_moveInput != Vector2.zero)
			{
				if (IsGrounded() && _moveInput.y < 0)
					_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;
				
				else
					_lastDashDir = _moveInput;
			}
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;

			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}

		#endregion

		#region Update: Slide

		IsSliding = CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0));

		/*if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
			IsSliding = true;
		else
			IsSliding = false;*/
		
		#endregion

		#region Update: Gravity

		if (!_isDashAttacking)
		{
			//Higher gravity if we've released the jump input or are falling
			if (IsSliding)
			{
				SetGravityScale(0);
			}

			else if (RB.velocity.y < 0 && _moveInput.y < 0)
			{
				//Much higher gravity if holding down
				SetGravityScale(gravityScale * fastFallGravityMult);
				//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFastFallSpeed));
			}

			else if (_isJumpCut || playerController.Stunned)
			{
				//Higher gravity if jump button released or the player is stunned
				SetGravityScale(gravityScale * jumpCutGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFallSpeed));
			}

			else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold)
			{
				SetGravityScale(gravityScale * jumpHangGravityMult);
			}

			else if (RB.velocity.y < 0)
			{
				//Higher gravity if falling
				SetGravityScale(gravityScale * fallGravityMult);
				//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFallSpeed));
			}

			else
			{
				//Default gravity if standing on a platform or moving upwards
				SetGravityScale(gravityScale);
			}
		}
		else
		{
			//No gravity when dashing (returns to normal once initial dashAttack phase over)
			SetGravityScale(0);
		}
		
		#endregion
    }

    public void onFixedUpdate()
	{
		#region FixedUpdate: Run
		
		//Handle Run
		if (!IsDashing)
		{
			if (IsWallJumping)
				Run(wallJumpRunLerp);
			else
				Run(1);
		}
		else if (_isDashAttacking)
		{
			Run(dashEndRunLerp);
		}

		#endregion

		#region FixedUpdate: Slide

		//Handle Slide
		if (IsSliding)
			Slide();

		#endregion
    }

    #region OnInput Callbacks
	//Methods which handle input detected in Update()

    public void OnJumpInput()
	{
		LastPressedJumpTime = jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}

	public void OnDashInput()
	{
		LastPressedDashTime = dashInputBufferTime;
	}

    #endregion

    #region Run Methods

    private void Run(float lerpAmount)
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = _moveInput.x * runMaxSpeed;

		//We can reduce our control using Lerp(), this smooths changes to our direction and speed
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);
		
		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're airborne.
		float accelRate;

		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * onAirReduction : runDeccelAmount * onAirReduction;
		
		//Increase our acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold)
		{
			accelRate *= jumpHangAccelerationMult;
			targetSpeed *= jumpHangMaxSpeedMult;
		}
		
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if(doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			//Prevent any deceleration from happening or, in other words, conserve our current momentum
			//You could experiment with allowing for the player to slightly increase their speed whilst in this "state"
			accelRate = 0; 
		}

		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - RB.velocity.x;

		//Calculate force along x-axis to apply to the player
		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}

    #endregion

    #region Jump Methods

    private void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		
		//We increase the force applied if we are falling
		//This means we'll always feel like we jump the same amount
		float force = jumpForce;
		if (RB.velocity.y < 0)
			force -= RB.velocity.y;

		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        playerController.jumpSound();
	}

	private void WallJump(int dir)
	{
		//Ensures we can't call Wall Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallRightTime = 0;
		LastOnWallLeftTime = 0;
		
		Vector2 force = new Vector2(wallJumpForce.x, wallJumpForce.y);
		force.x *= dir; //apply force in opposite direction of wall

		if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
			force.x -= RB.velocity.x;

		if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity)
			force.y -= RB.velocity.y; //This ensures the player always reaches our desired jump force or greater

		//Unlike in the run we want to use the Impulse mode.
		//The default mode will apply our force instantly ignoring mass
		RB.AddForce(force, ForceMode2D.Impulse);
	}

	#endregion

	#region Dash Methods

	//Dash Coroutine
	private IEnumerator StartDash(Vector2 dir)
	{
		//Overall this method of dashing aims to mimic Celeste
		LastOnGroundTime = 0;
		LastPressedDashTime = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_isDashAttacking = true;

		SetGravityScale(0);

		//We keep the player's velocity at the dash speed during the "attack" phase
		while (Time.time - startTime <= dashAttackTime)
		{
			RB.velocity = dir.normalized * dashSpeed;
			//Pauses the loop until the next frame, creating something of an Update loop. 
			//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
			yield return null;
		}

		startTime = Time.time;

		_isDashAttacking = false;

		//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
		SetGravityScale(gravityScale);
		RB.velocity = dashEndSpeed * dir.normalized;

		while (Time.time - startTime <= dashEndTime)
		{
			yield return null;
		}

		//Dash over
		IsDashing = false;
	}

	//Short period before the player is able to dash again
	private IEnumerator RefillDash(int amount)
	{
		//Short cooldown, so we can't constantly dash along the ground
		_dashRefilling = true;
		yield return new WaitForSeconds(dashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(dashAmount, _dashesLeft + 1);
	}

	#endregion

	#region Slide Methods

	private void Slide()
	{
		//We remove the remaining upwards Impulse to prevent upwards sliding
		if(RB.velocity.y > 0)
		{
		    RB.AddForce(-RB.velocity.y * Vector2.up, ForceMode2D.Impulse);
		}
	
		//Works the same as the Run but only in the y-axis
		float speedDif = slideSpeed - RB.velocity.y;	
		float movement = speedDif * slideAccel;

		//So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
		//The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		RB.AddForce(movement * Vector2.up);
	}

    #endregion


    #region Check Methods

    private bool CanJump()
    {
		return LastOnGroundTime > 0 && !IsJumping;
    }

	private bool CanWallJump()
    {
		return canWallJump && LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
			 (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
	}

	private bool CanJumpCut()
    {
		return IsJumping && RB.velocity.y > 0;
    }

	private bool CanWallJumpCut()
	{
		return canWallJump && IsWallJumping && RB.velocity.y > 0;
	}

	private bool CanDash()
	{
		if (!IsDashing && _dashesLeft < dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
		{
			StartCoroutine(nameof(RefillDash), 1);
		}

		return _dashesLeft > 0;
	}

	public bool CanSlide()
    {
		if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
			return true;
		else
			return false;
	}

    #endregion

	#region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
	}
    #endregion

    #region Region de ejemplo

    // brrr

    #endregion
}
