using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
	public float Duration;
	public float Velocity;

	private float _timer;

	private void Start()
	{
		_timer = Duration;
	}

	private void Update()
	{
		transform.Translate(0, Velocity * Time.deltaTime, 0);

		_timer -= Time.deltaTime;

		if (_timer < 0)
		{
			Destroy(gameObject);
		}
	}
}
