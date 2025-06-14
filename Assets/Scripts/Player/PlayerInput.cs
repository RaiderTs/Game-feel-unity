using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
	public FrameInput FrameInput { get; private set; }

	private PlayerInputActions _playerInputActions;
	private InputAction _move;
	private InputAction _jump;

	private void Awake()
	{
		_playerInputActions = new PlayerInputActions();
		_move = _playerInputActions.Player.Move;
		_jump = _playerInputActions.Player.Jump;
	}

	private void OnEnable() // запускается при активации скрипта
	{
		_playerInputActions.Enable();
	}

	private void OnDisable() // запускается при деактивации скрипта
	{
		_playerInputActions.Disable();
	}

	private void Update()
	{
	  FrameInput =	GatherInput();
	}

	private FrameInput GatherInput()
	{
		return new FrameInput
		{
			Move = _move.ReadValue<Vector2>(),
			Jump = _jump.WasPressedThisFrame(),
		};
	}
}


public struct FrameInput
{
	public Vector2 Move;
	public bool Jump;
}
