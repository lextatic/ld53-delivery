using UnityEngine;

public class PlayAudioOnDestroy : MonoBehaviour
{
	public AudioSource AudioSource;
	public SimpleAudioEvent AudioEvent;

	private void Start()
	{
		AudioSource = GameObject.FindGameObjectWithTag("AudioSource").GetComponent<AudioSource>();
	}

	private void OnDestroy()
	{
		if (AudioSource != null)
		{
			AudioEvent.Play(AudioSource);
		}
	}
}
