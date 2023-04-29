using System;
using UnityEngine;

public class Table : MonoBehaviour
{
	public int TableID;
	public Rigidbody2D Drone;
	public bool HasActiveOrder;

	private bool _droneIsAtTable;
	private DroneContainer _droneContainer;

	public event Action<Table> OnOrderDelivered;

	private void Awake()
	{
		_droneIsAtTable = false;
		_droneContainer = Drone.GetComponent<DroneContainer>();
	}

	private void Update()
	{
		if (HasActiveOrder && _droneIsAtTable && Drone.IsSleeping() && _droneContainer.DeliverableList.Count != 0)
		{
			Debug.Log("At table!!");

			HasActiveOrder = false;

			var score = CalculateScore();

			Debug.Log(score);

			OnOrderDelivered.Invoke(this);

			foreach (var item in _droneContainer.DeliverableList)
			{
				Destroy(item.gameObject);
			}

			_droneContainer.DeliverableList.Clear();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == Drone.gameObject)
		{
			_droneIsAtTable = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject == Drone.gameObject)
		{
			_droneIsAtTable = false;
		}
	}

	private int CalculateScore()
	{
		int totalScore = 0;

		foreach (var item in _droneContainer.DeliverableList)
		{
			if (Physics2D.Raycast(item.transform.position, Vector2.down, 5f, 1 << LayerMask.NameToLayer("Score")))
			{
				if (Vector3.Angle(item.transform.up, Vector3.up) <= 15f)
				{
					totalScore += item.GetComponent<DeliverableScore>().Score;
				}
			}
		}


		return totalScore;
	}
}
