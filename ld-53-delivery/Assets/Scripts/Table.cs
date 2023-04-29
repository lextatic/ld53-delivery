using System;
using UnityEngine;

public class Table : MonoBehaviour
{
	public int TableID;
	public Rigidbody2D Drone;
	public Transform VisualIndicator;

	private bool _droneIsAtTable;
	private DroneContainer _droneContainer;
	private bool _hasActiveOrder;

	public event Action<Table> OnOrderDelivered;

	public bool HasActiveOrder
	{
		set
		{
			_hasActiveOrder = value;
			VisualIndicator.gameObject.SetActive(value);
		}

		get => _hasActiveOrder;
	}

	private void Awake()
	{
		_droneIsAtTable = false;
		_droneContainer = Drone.GetComponent<DroneContainer>();
	}

	private void Update()
	{
		if (HasActiveOrder)
		{
			UpdateIndicator();

			if (_droneIsAtTable && Drone.IsSleeping() && _droneContainer.DeliverableList.Count != 0)
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

	private void UpdateIndicator()
	{
		if (Vector3.Distance(Drone.transform.position, transform.position) > 20)
		{
			VisualIndicator.transform.position = Drone.transform.position + (transform.position - Drone.transform.position).normalized * 20;
		}
		else
		{
			VisualIndicator.transform.position = transform.position;
		}
	}
}
