using UnityEngine;

public class Controller : MonoBehaviour
{
	public Transform LeftMotor;
	public Transform RightMotor;
	public float MotorForce;

	private Rigidbody2D _myRigidBody;


	private void Start()
	{
		_myRigidBody = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			_myRigidBody.AddForceAtPosition(transform.up * MotorForce, LeftMotor.position, ForceMode2D.Force);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			_myRigidBody.AddForceAtPosition(transform.up * MotorForce, RightMotor.position, ForceMode2D.Force);
		}
	}
}
