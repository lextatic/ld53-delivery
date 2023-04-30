using TMPro;
using UnityEngine;

public class DeliverableScore : MonoBehaviour
{
	[SerializeField]
	private int _score;

	public TextMeshPro ScoreTextPrefab;

	private bool _wasUsed;

	public int ReadScore()
	{
		_wasUsed = true;
		return _score;
	}

	public void OnDestroy()
	{
		if (!_wasUsed)
		{
			return;
		}

		var scorePrefab = Instantiate(ScoreTextPrefab, transform.position, Quaternion.identity);

		if (_score >= 0)
		{
			scorePrefab.text = $"+{_score}";
		}
		else
		{
			scorePrefab.color = Color.red;
			scorePrefab.text = $"{_score}";
		}
	}
}
