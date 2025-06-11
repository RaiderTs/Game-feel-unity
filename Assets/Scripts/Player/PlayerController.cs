using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static Action OnJump;

	public static PlayerController Instance;

	[SerializeField] private Transform _feetTransform;
	[SerializeField] private Vector2 _groundCheck;
	[SerializeField] private LayerMask _groundLayer;
	[SerializeField] private float _jumpStrength = 7f;
	[SerializeField] private float _extraGravity = 700f;
	[SerializeField] private float _gravityDelay = 0.2f; // задержка отскока от земли
	[SerializeField] private float _coyoteTime = 0.5f; // время нахождения на земле


	private float _timeInAir, _coyoteTimer; // время в воздухе
	private bool _doubleJumpAvailable; // доступность двойного прыжка

	private PlayerInput _playerInput;
	private FrameInput _frameInput;

	private Rigidbody2D _rigidBody;
	private Movement _movement;


	public void Awake()
	{
		if (Instance == null) { Instance = this; }

		_rigidBody = GetComponent<Rigidbody2D>();
		_playerInput = GetComponent<PlayerInput>(); // получаем компонент PlayerInput
		_movement = GetComponent<Movement>(); // получаем компонент Movement
	}

	// подписываемся на событие
	private void OnEnable()
	{
		OnJump += ApplyJumpForce;
	}

	private void OnDisable()
	{
		OnJump -= ApplyJumpForce;
	}


	private void Update()
	{
		GatherInput();
		Movement();
		CoyoteTimer();
		HandleJump();
		HandleSpriteFlip();
		GravityDelay();
	}

	private void FixedUpdate()
	{
		ExtraGravity();
	}

	public bool IsFacingRight()
	{
		return transform.eulerAngles.y == 0;
	}


	private bool CheckGrounded()
	{
		Collider2D isGrounded = Physics2D.OverlapBox(_feetTransform.position, _groundCheck, 0f, _groundLayer);
		return isGrounded;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(_feetTransform.position, _groundCheck);
	}


	private void GravityDelay() // задержка отскока
	{
		if (!CheckGrounded())
		{
			_timeInAir += Time.deltaTime;
		}
		else
		{
			_timeInAir = 0f;
		}
	}

	private void ExtraGravity() // дополнительная гравитация
	{
		if (_timeInAir > _gravityDelay)
		{
			_rigidBody.AddForce(new Vector2(0f, -_extraGravity * Time.deltaTime)); // дополнительная гравитация
		}
	}

	private void GatherInput()
	{
		_frameInput = _playerInput.FrameInput;
	}

	private void Movement()
	{
		_movement.SetCurrentDirection(_frameInput.Move.x); // устанавливаем направление движения
	}

	private void HandleJump()
	{
		if (!_frameInput.Jump)
		{
			return;
		}

		if (CheckGrounded())     // Прыжок с земли (обычный прыжок)
		{
			OnJump?.Invoke();
		}
		else if (_coyoteTimer > 0f)   // Coyote time прыжок (прыжок в течение короткого времени после покидания земли)
		{
			OnJump?.Invoke();
			_doubleJumpAvailable = true;
		}
		else if (_doubleJumpAvailable)  // Двойной прыжок (только если доступен и мы в воздухе)
		{
			OnJump?.Invoke();
			_doubleJumpAvailable = false;
		}
	}

	private void CoyoteTimer()
	{
		if (CheckGrounded())
		{
			_coyoteTimer = _coyoteTime;
			_doubleJumpAvailable = true; // доступность двойного прыжка
		}
		else
		{
			_coyoteTimer -= Time.deltaTime; // уменьшаем время нахождения на земле
		}
	}

	private void ApplyJumpForce()
	{
		_rigidBody.velocity = Vector2.zero; // обнуляем скорость перед прыжком
		_timeInAir = 0f; // обнуляем время в воздухе
		_coyoteTimer = 0f; // обнуляем время нахождения на земле
		_rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
	}

	private void HandleSpriteFlip()
	{
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (mousePosition.x < transform.position.x)
		{
			transform.eulerAngles = new Vector3(0f, -180f, 0f);
		}
		else
		{
			transform.eulerAngles = new Vector3(0f, 0f, 0f);
		}
	}
}
