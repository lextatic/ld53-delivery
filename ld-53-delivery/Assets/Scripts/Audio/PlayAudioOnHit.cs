using UnityEngine;

public class PlayAudioOnHit : MonoBehaviour
{
	public SimpleAudioEvent AudioEvent;
	public float Cooldown;

	private AudioSource _audioSource;

	private float _cooldownTimer;

	private void Start()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		_cooldownTimer -= Time.deltaTime;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (_audioSource != null && collision.relativeVelocity.magnitude > 0.1f && _cooldownTimer <= 0)
		{
			AudioEvent.Play(_audioSource);
			_cooldownTimer = Cooldown;
		}
	}
}
