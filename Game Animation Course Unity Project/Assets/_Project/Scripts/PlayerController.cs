using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using Prime31.StateKitLite;
using static UnityEngine.InputSystem.InputAction;

public enum PlayerMovementStates
{
	None,
	Idle,
	Walking,
	Jumping,
	Falling,
	Hurt,
	Dead
}

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Actor))]
public class PlayerController : StateKitLite<PlayerMovementStates>
{
	[Header("Misc References")]
	public CharacterController2D characterController2d;
	private Actor actor;

	[Header("Input")]
	public Vector2 movementInput;
	public Vector2 modifiedMovementInput;
	public bool jumpInput;
	public bool shouldWalk;
	PlayerInputActions playerInputActions;

	[Header("Movement")]
	public float Gravity = 20f;
	public float MovementSpeed = 10;

	[Range(.1f, .49f)]
	public float walkSpeedModifier = .4f;

	public float Deacceleration = .1f;
	public float DeaccelerationVol = 0f;
	public Vector2 velocity;

	public float UncontrollableDeacceleration = .1f;
	public float UncontrollableDeaccelerationVol = 0f;

	[Header("Jump")]
	public float JumpSpeed = 1;
	public bool jump_clear = true;

	[Header("Stats")]
	public int HP;
	bool isDead = false;

	[Header("Art")]
	public Transform characterBody;

	[Header("Animation")]
	public Animator animator;
	public string speedProperty = "speed";
	public string measuredVelocityProperty = "measuredVelocity";
	public string verticalSpeedProperty = "vertical";
	public string groundedProperty = "grounded";
	public string deadProperty = "dead";

	protected override void Awake()
	{
		base.Awake();

		actor = GetComponent<Actor>();
		actor.OnDie += HandleDeath;

		characterController2d = GetComponent<CharacterController2D>();

		playerInputActions = new PlayerInputActions();
		playerInputActions.Player.Move.performed += HandleMovePerformed;
		playerInputActions.Player.Move.canceled += HandleMoveCanceled;

		playerInputActions.Player.Jump.started += ctx => jumpInput = true;
		playerInputActions.Player.Jump.canceled += ctx => jumpInput = false;

		playerInputActions.Player.Walk.started += ctx => shouldWalk = true;
		playerInputActions.Player.Walk.canceled += ctx => shouldWalk = false;
	}

	public void Start()
	{
		initialState = PlayerMovementStates.Idle;
	}

	private bool isMovePerformed = false;
	public void HandleMovePerformed(CallbackContext ctx)
	{
		isMovePerformed = true;
		movementInput = ctx.ReadValue<Vector2>();
	}

	public void HandleMoveCanceled(CallbackContext ctx)
	{
		isMovePerformed = false;
	}

	protected override void Update()
	{
		base.Update();

		modifiedMovementInput = movementInput;
		if (shouldWalk)
		{
			modifiedMovementInput.x *= walkSpeedModifier;
		}

		if (velocity.x > 0 && characterBody.eulerAngles.y != 0)
		{
			characterBody.eulerAngles = Vector3.up * 0;
		}
		else if (velocity.x < 0 && characterBody.eulerAngles.y != 180)
		{
			characterBody.eulerAngles = Vector3.up * 180;
		}

		if (characterController2d.isGrounded && velocity.y < 0)
		{
			velocity.y = -.1f;
		}

		animator.SetFloat(speedProperty, Mathf.Abs(modifiedMovementInput.x));
		animator.SetFloat(measuredVelocityProperty, Mathf.Abs(velocity.x));
		animator.SetFloat(verticalSpeedProperty, velocity.y);
		animator.SetBool(groundedProperty, characterController2d.isGrounded);
	}

	private void OnEnable()
	{
		if (playerInputActions != null)
		{
			playerInputActions.Enable();
		}
	}

	private void OnDisable()
	{
		if (playerInputActions != null)
		{
			playerInputActions.Disable();
		}
	}

	// StateKitLite States
	void None_Enter() {}
	void None_Tick() {}
	void None_Exit() {}

	// Idle
	void Idle_Enter() {}
	void Idle_Tick()
	{

		ApplyGravity();
		DoVelocityMovement();

		if (CheckForJump())
		{
			return;
		}
		if (CheckForFall())
		{
			return;
		}

		HandleXMovement();
		if (Mathf.Abs(movementInput.x) > 0)
		{
			currentState = PlayerMovementStates.Walking;
			return;
		}

	}
	void Idle_Exit() {}

	// Walking
	void Walking_Enter() {}
	void Walking_Tick()
	{

		ApplyGravity();
		DoVelocityMovement();

		if (CheckForJump())
		{
			return;
		}
		if (CheckForFall())
		{
			return;
		}

		HandleXMovement();
		if (movementInput.x == 0)
		{
			currentState = PlayerMovementStates.Idle;
			return;
		}
	}
	void Walking_Exit() {}

	// Jumping
	void Jumping_Enter()
	{
		velocity.y = JumpSpeed;
	}

	void Jumping_Tick()
	{

		if (!jumpInput)
		{
			velocity.y = 0;
			currentState = PlayerMovementStates.Falling;
			return;
		}

		if (velocity.y <= 0)
		{
			currentState = PlayerMovementStates.Falling;
			return;
		}

		ApplyGravity();
		HandleXMovement();
		DoVelocityMovement();
	}
	void Jumping_Exit() {}

	// Falling
	void Falling_Enter()
	{
		if (velocity.y < 0)
		{
			velocity.y = 0;
		}
	}

	void Falling_Tick()
	{

		// if hits head, start falling
		if (velocity.y > 0 && characterController2d.collisionState.above)
		{
			velocity.y = 0;
		}

		if (characterController2d.isGrounded)
		{
			currentState = PlayerMovementStates.Idle;
			return;
		}

		ApplyGravity();
		HandleXMovement();
		DoVelocityMovement();

	}
	void Falling_Exit() {}

	// Hurt
	void Hurt_Enter() {}

	void Hurt_Tick()
	{
		DoVelocityMovement();
		ApplyGravity();
	}

	void Hurt_Exit() {}

	// Dead
	void Dead_Enter() {}

	void Dead_Tick()
	{
		DoVelocityMovement();
		ApplyGravity();
	}

	void Dead_Exit() {}

	public bool CheckForJump()
	{
		if (jumpInput)
		{
			currentState = PlayerMovementStates.Jumping;
			return true;
		}
		return false;
	}

	public bool CheckForFall()
	{
		// if not grounded, fall
		if (!characterController2d.isGrounded)
		{
			currentState = PlayerMovementStates.Falling;
			return true;
		}
		return false;
	}

	public void ApplyGravity()
	{
		velocity.y -= Gravity * Time.deltaTime;
	}

	public void HandleGroundedVerticalVelocity()
	{
		if (characterController2d.isGrounded)
		{
			velocity.y = -.01f;
		}
	}

	Vector3 _uncontrollableVelocity;
	private void HandleXMovement()
	{
		if (movementInput.x == 0)
		{
			velocity.x = Mathf.SmoothDamp(velocity.x, 0 + _uncontrollableVelocity.x, ref DeaccelerationVol, Deacceleration);
		}
		else
		{
			velocity.x = Mathf.SmoothDamp(velocity.x, (modifiedMovementInput.x * MovementSpeed) + _uncontrollableVelocity.x, ref DeaccelerationVol, Deacceleration);
		}
		_uncontrollableVelocity.x = Mathf.SmoothDamp(_uncontrollableVelocity.x, 0, ref UncontrollableDeaccelerationVol, UncontrollableDeacceleration);
	}

	public void DoVelocityMovement()
	{
		characterController2d.Move(velocity * Time.deltaTime);
	}

	public void HandleDeath()
	{
		velocity = Vector2.zero;
		currentState = PlayerMovementStates.Dead;
		animator.SetTrigger(deadProperty);
		GameManager.Instance.HandleLose();
	}

}
