using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public TextMeshProUGUI TimerLabel;

	public float CurrentTimer { get; private set; }

	private void Start()
	{
		CurrentTimer = 0;
	}

	private void Update()
	{
		CurrentTimer += Time.deltaTime;

		TimerLabel.text = FormatTime(CurrentTimer);
	}

	private string FormatTime(float timeInSeconds)
	{
		int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
		int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
		return string.Format("{0:00}:{1:00}", minutes, seconds);
	}
}
