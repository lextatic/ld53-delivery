using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
	public Transform LeftMotor;
	public Transform RightMotor;
	public float MotorForce;

	public GameObject MenuPanel;
	public GameObject ModalPanel;
	public GameObject VictorylPanel;
	public Button PauseButton;
	public Button RetryButton;
	public Button ResumeButton;
	public Button RestartButton;
	public Button QuitButton;
	public Button YesButton;
	public Button PlayAgainButton;
	public Toggle SwapControls;

	public AnimationCurve ThrottleControlCurve;

	public AudioSource AudioSourceLeft;
	public AudioSource AudioSourceRight;

	public ParticleSystem ParticleSystemLeft;
	public ParticleSystem ParticleSystemRight;

	private GameActions _gameActions;
	private InputAction _leftThrottleAction;
	private InputAction _rightThrottleAction;
	private InputAction _retryAction;
	private InputAction _menuAction;

	private Rigidbody2D _myRigidBody;

	private string _sceneToLoad;
	private bool _controlsSwapped;

	public float LeftThrottle { get; private set; }
	public float RightThrottle { get; private set; }

	private void Awake()
	{
		_myRigidBody = GetComponent<Rigidbody2D>();

		_gameActions = new GameActions();

		_leftThrottleAction = _gameActions.PlayerMovement.LeftThrottle;
		_rightThrottleAction = _gameActions.PlayerMovement.RightThrottle;
		_retryAction = _gameActions.UI.Retry;
		_menuAction = _gameActions.UI.Menu;
		_controlsSwapped = false;

		_controlsSwapped = PlayerPrefs.GetInt("SwapControls", 0) != 0;
		SwapControls.isOn = _controlsSwapped;

		Input.multiTouchEnabled = true;
	}

	private void OnEnable()
	{
		_leftThrottleAction.Enable();
		_rightThrottleAction.Enable();
		_retryAction.Enable();
		_menuAction.Enable();

		_retryAction.performed += RestartAction_Performed;
		_menuAction.performed += MenuAction_Performed;
	}

	private void MenuAction_Performed(InputAction.CallbackContext obj)
	{
		if (!MenuPanel.activeInHierarchy)
		{
			Pause();
		}
		else
		{
			Resume();
		}
	}

	public void Pause()
	{
		MenuPanel.SetActive(true);
		Time.timeScale = 0f;

		PauseButton.interactable = false;
		RetryButton.interactable = false;

		if (Input.GetJoystickNames().Length > 0)
		{
			ResumeButton.Select();
		}
	}

	public void Resume()
	{
		MenuPanel.SetActive(false);
		Time.timeScale = 1f;

		PauseButton.interactable = true;
		RetryButton.interactable = true;
	}

	public void Restart()
	{
		_sceneToLoad = "GameScene";

		ShowModal();
	}

	public void ConfirmModal()
	{
		SceneManager.LoadScene(_sceneToLoad);
		Time.timeScale = 1f;
	}

	public void CancelModal()
	{
		ModalPanel.SetActive(false);

		ResumeButton.interactable = true;
		RestartButton.interactable = true;
		QuitButton.interactable = true;
		SwapControls.interactable = true;

		if (Input.GetJoystickNames().Length > 0)
		{
			ResumeButton.Select();
		}
	}

	public void Quit()
	{
		_sceneToLoad = "MainMenu";

		ShowModal();
	}

	public void ShowVictoryPanel()
	{
		VictorylPanel.SetActive(true);
		Time.timeScale = 0f;

		PauseButton.interactable = false;
		RetryButton.interactable = false;

		if (Input.GetJoystickNames().Length > 0)
		{
			PlayAgainButton.Select();
		}
	}

	public void PlayAgain()
	{
		SceneManager.LoadScene("GameScene");
		Time.timeScale = 1f;
	}

	public void VictoryQuit()
	{
		SceneManager.LoadScene("MainMenu");
		Time.timeScale = 1f;
	}

	private void ShowModal()
	{
		ModalPanel.SetActive(true);

		ResumeButton.interactable = false;
		RestartButton.interactable = false;
		QuitButton.interactable = false;
		SwapControls.interactable = false;

		if (Input.GetJoystickNames().Length > 0)
		{
			YesButton.Select();
		}
	}

	public void OnSwapControls()
	{
		_controlsSwapped = SwapControls.isOn;
		PlayerPrefs.SetInt("SwapControls", _controlsSwapped ? 1 : 0);
	}

	private void RestartAction_Performed(InputAction.CallbackContext obj)
	{
		Retry();
	}

	public void Retry()
	{
		FindObjectOfType<KitchenCounter>().LoadCheckpoint();
	}

	private void OnDisable()
	{
		_leftThrottleAction.Disable();
		_rightThrottleAction.Disable();
		_retryAction.Disable();
		_menuAction.Enable();

		_retryAction.performed -= RestartAction_Performed;
		_menuAction.performed -= MenuAction_Performed;
	}

	private void Update()
	{
		LeftThrottle = _leftThrottleAction.ReadValue<float>();
		RightThrottle = _rightThrottleAction.ReadValue<float>();

		foreach (var touch in Input.touches)
		{
			if (touch.position.x < Screen.width / 2)
			{
				LeftThrottle = 1;
			}
			else if (touch.position.x > Screen.width / 2)
			{
				RightThrottle = 1;
			}
		}

		if (_controlsSwapped)
		{
			var swapValue = LeftThrottle;
			LeftThrottle = RightThrottle;
			RightThrottle = swapValue;
		}

		// 0.8 - 1.2
		AudioSourceLeft.pitch = 0.8f + (LeftThrottle * 0.4f);
		// 0.8 - 1.2
		AudioSourceRight.pitch = 0.8f + (RightThrottle * 0.4f);

		var main = ParticleSystemLeft.main;
		main.startSpeed = 1 + (10 * LeftThrottle);
		main = ParticleSystemRight.main;
		main.startSpeed = 1 + (10 * RightThrottle);
	}

	private void FixedUpdate()
	{
		var adjustedLeftThrottle = ThrottleControlCurve.Evaluate(LeftThrottle);
		var adjustedRightThrottle = ThrottleControlCurve.Evaluate(RightThrottle);

		_myRigidBody.AddForceAtPosition(adjustedLeftThrottle * transform.up * MotorForce, LeftMotor.position, ForceMode2D.Force);
		_myRigidBody.AddForceAtPosition(adjustedRightThrottle * transform.up * MotorForce, RightMotor.position, ForceMode2D.Force);
	}
}
