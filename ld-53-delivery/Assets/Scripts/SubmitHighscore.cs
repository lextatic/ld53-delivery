using Dan.Main;
using TMPro;
using UnityEngine;

public class SubmitHighscore : MonoBehaviour
{
	private readonly string _leaderboardPublicKey = "d28d4a4463678d91fed629cb6ac4b98551e74a25a6564a2ab88b2adb25fe3090";

	public TextMeshProUGUI ScoreTextLabel;
	public TMP_InputField NicknameInputField;
	public Timer Timer;
	public Score Score;

	private int _totalScore;

	public void Submit()
	{
		if (!string.IsNullOrEmpty(NicknameInputField.text))
		{
			LeaderboardCreator.UploadNewEntry(_leaderboardPublicKey, NicknameInputField.text, _totalScore);
		}
		else
		{
			Debug.LogError("Invalid nickname");
		}
	}

	private void OnEnable()
	{
		_totalScore = Score.CurrentScore + Mathf.Max(0, 600 - Mathf.RoundToInt(Timer.CurrentTimer));

		ScoreTextLabel.text = $"Work Score: {Score.CurrentScore}\n" +
			$"Time Bonus: {Mathf.Max(0, 600 - Mathf.RoundToInt(Timer.CurrentTimer))}\n" +
			$"TotalScore: {_totalScore}";
	}
}
