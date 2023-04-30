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
	public Button ResumeButton;
	public Button RestartButton;
	public Button QuitButton;
	public Button YesButton;
	public Button PlayAgainButton;

	public AnimationCurve ThrottleControlCurve;

	public AudioSource AudioSourceLeft;
	public AudioSource AudioSourceRight;
	//public AudioClip ThrottleSound;

	private GameActions _gameActions;
	private InputAction _leftThrottleAction;
	private InputAction _rightThrottleAction;
	private InputAction _retryAction;
	private InputAction _menuAction;

	private Rigidbody2D _myRigidBody;

	private string _sceneToLoad;

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

		PauseButton.enabled = false;

		if (Input.GetJoystickNames().Length > 0)
		{
			ResumeButton.Select();
		}
	}

	public void Resume()
	{
		MenuPanel.SetActive(false);
		Time.timeScale = 1f;

		PauseButton.enabled = true;
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

		ResumeButton.enabled = true;
		RestartButton.enabled = true;
		QuitButton.enabled = true;

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

		PauseButton.enabled = false;

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

		ResumeButton.enabled = false;
		RestartButton.enabled = false;
		QuitButton.enabled = false;

		if (Input.GetJoystickNames().Length > 0)
		{
			YesButton.Select();
		}
	}

	private void RestartAction_Performed(InputAction.CallbackContext obj)
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

		//AudioSource.volume = (LeftThrottle + RightThrottle) > 0 ? 1f : 0f;

		//// 0.8 - 1.4
		//AudioSource.pitch = 0.8f + ((LeftThrottle + RightThrottle) / 2f) * 0.6f;

		// 0.8 - 1.2
		AudioSourceLeft.pitch = 0.8f + (LeftThrottle * 0.4f);
		// 0.8 - 1.2
		AudioSourceRight.pitch = 0.8f + (RightThrottle * 0.4f);
	}

	private void FixedUpdate()
	{
		var adjustedLeftThrottle = ThrottleControlCurve.Evaluate(LeftThrottle);
		var adjustedRightThrottle = ThrottleControlCurve.Evaluate(RightThrottle);

		_myRigidBody.AddForceAtPosition(adjustedLeftThrottle * transform.up * MotorForce, LeftMotor.position, ForceMode2D.Force);
		_myRigidBody.AddForceAtPosition(adjustedRightThrottle * transform.up * MotorForce, RightMotor.position, ForceMode2D.Force);
	}
}
