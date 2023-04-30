using UnityEngine;

public class FloatEffect : MonoBehaviour
{
	public float Amplitude = 0.5f;
	public float Frequency = 1f;
	public float Delay = 0f;

	private Vector3 startPos;

	private void Start()
	{
		startPos = transform.position;
	}

	private void Update()
	{
		transform.position = startPos + Vector3.up * Mathf.Sin(Time.time + Delay * Frequency) * Amplitude;
	}
}