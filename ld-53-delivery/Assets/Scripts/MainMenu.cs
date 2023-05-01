using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public Button StartGameButton;

	private void Start()
	{
		if (Input.GetJoystickNames().Length > 0)
		{
			StartGameButton.Select();
		}
	}

	public void StartGame()
	{
		SceneManager.LoadScene("GameScene");
	}
}
