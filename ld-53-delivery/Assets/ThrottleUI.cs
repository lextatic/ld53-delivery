using UnityEngine;
using UnityEngine.UI;

public class ThrottleUI : MonoBehaviour
{
	public enum Motor
	{
		Left,
		Right
	}

	public Controller Controller;
	public Image FillSprite;
	public Motor TargetMotor;

	private void Update()
	{
		if (TargetMotor == Motor.Left)
		{
			FillSprite.fillAmount = Controller.LeftThrottle;
		}
		else
		{
			FillSprite.fillAmount = Controller.RightThrottle;
		}
	}
}
