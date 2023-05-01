using Dan.Main;
using Dan.Models;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
	public List<TextMeshProUGUI> Players = new List<TextMeshProUGUI>();
	public List<TextMeshProUGUI> Scores = new List<TextMeshProUGUI>();

	private readonly string _leaderboardPublicKey = "d28d4a4463678d91fed629cb6ac4b98551e74a25a6564a2ab88b2adb25fe3090";

	public void Start()
	{
		GetLeaderboard();
	}

	private void GetLeaderboard()
	{
		LeaderboardCreator.GetLeaderboard(_leaderboardPublicKey, true,
			new LeaderboardSearchQuery
			{
				Skip = 0,
				Take = 10,

			},
			msg =>
			{
				int count = 0;

				Debug.Log(msg.Length);

				foreach (var item in msg)
				{
					Players[count].text = item.Username;
					Scores[count].text = item.Score.ToString();
					count++;
				}
			});
	}
}
