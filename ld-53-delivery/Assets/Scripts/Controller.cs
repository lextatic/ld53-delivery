using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
	public Transform LeftMotor;
	public Transform RightMotor;
	public float MotorForce;

	public AnimationCurve ThrottleControlCurve;

	private GameActions _gameActions;
	private InputAction _leftThrottleAction;
	private InputAction _rightThrottleAction;
	private InputAction _restartAction;

	private Rigidbody2D _myRigidBody;

	public float LeftThrottle { get; private set; }
	public float RightThrottle { get; private set; }

	private void Awake()
	{
		_myRigidBody = GetComponent<Rigidbody2D>();

		_gameActions = new GameActions();

		_leftThrottleAction = _gameActions.PlayerMovement.LeftThrottle;
		_rightThrottleAction = _gameActions.PlayerMovement.RightThrottle;
		_restartAction = _gameActions.UI.Restart;
	}

	private void OnEnable()
	{
		_leftThrottleAction.Enable();
		_rightThrottleAction.Enable();
		_restartAction.Enable();

		_restartAction.performed += RestartAction_Performed;
	}

	private void RestartAction_Performed(InputAction.CallbackContext obj)
	{
		FindObjectOfType<KitchenCounter>().LoadCheckpoint();
	}

	private void OnDisable()
	{
		_leftThrottleAction.Disable();
		_rightThrottleAction.Disable();
		_restartAction.Disable();
	}

	private void Update()
	{
		LeftThrottle = _leftThrottleAction.ReadValue<float>();
		RightThrottle = _rightThrottleAction.ReadValue<float>();
	}

	private void FixedUpdate()
	{
		var adjustedLeftThrottle = ThrottleControlCurve.Evaluate(LeftThrottle);
		var adjustedRightThrottle = ThrottleControlCurve.Evaluate(RightThrottle);

		_myRigidBody.AddForceAtPosition(adjustedLeftThrottle * transform.up * MotorForce, LeftMotor.position, ForceMode2D.Force);
		_myRigidBody.AddForceAtPosition(adjustedRightThrottle * transform.up * MotorForce, RightMotor.position, ForceMode2D.Force);
	}
}
