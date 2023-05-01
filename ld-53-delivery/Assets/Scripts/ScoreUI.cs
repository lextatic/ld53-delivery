using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
	public Score Score;
	public TextMeshProUGUI ScoreText;

	//private int HighScore;

	private void Start()
	{
		Score.CurrentScore = 0;

		//HighScore = PlayerPrefs.GetInt("HighScore", 0);
	}

	private void Update()
	{
		ScoreText.text = $"<color=#FFB100>Score:</color> {Score.CurrentScore}";
	}
}
