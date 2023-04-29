using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
	public Transform LeftMotor;
	public Transform RightMotor;
	public float MotorForce;

	private GameActions _gameActions;

	private InputAction _leftThrottleAction;
	private InputAction _rightThrottleAction;

	private Rigidbody2D _myRigidBody;

	private void Awake()
	{
		_myRigidBody = GetComponent<Rigidbody2D>();

		_gameActions = new GameActions();

		_leftThrottleAction = _gameActions.PlayerMovement.LeftThrottle;
		_rightThrottleAction = _gameActions.PlayerMovement.RightThrottle;
	}

	private void OnEnable()
	{
		_leftThrottleAction.Enable();
		_rightThrottleAction.Enable();
	}

	private void OnDisable()
	{
		_leftThrottleAction.Disable();
		_rightThrottleAction.Disable();
	}

	private void FixedUpdate()
	{
		_myRigidBody.AddForceAtPosition(_leftThrottleAction.ReadValue<float>() * transform.up * MotorForce, LeftMotor.position, ForceMode2D.Force);
		_myRigidBody.AddForceAtPosition(_rightThrottleAction.ReadValue<float>() * transform.up * MotorForce, RightMotor.position, ForceMode2D.Force);
	}
}
