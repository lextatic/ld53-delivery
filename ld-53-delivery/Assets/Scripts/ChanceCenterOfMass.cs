using UnityEngine;

public class ChanceCenterOfMass : MonoBehaviour
{
	public float Ammount;

	private void Start()
	{
		var rigidbody = GetComponent<Rigidbody2D>();
		rigidbody.centerOfMass = rigidbody.centerOfMass - (Vector2.up * Ammount);
	}
}
