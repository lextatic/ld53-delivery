using System;
using UnityEngine;

public class Table : MonoBehaviour
{
	public int TableID;
	public Rigidbody2D Drone;
	public Transform VisualIndicator;

	private bool _droneIsAtTable;
	private DroneContainer _droneContainer;
	private Collider2D _tableCollider;
	private bool _hasActiveOrder;
	private bool _hasPendingDishes;
	private GameObject _dishesPrefab;

	public event Action<Table> OnOrderDelivered;
	public event Action<Table> OnDishesCollected;

	public bool HasActiveOrder
	{
		set
		{
			_hasActiveOrder = value;
			VisualIndicator.gameObject.SetActive(value);
			_tableCollider.enabled = value;
		}

		get => _hasActiveOrder;
	}

	public bool HasPendingDishes
	{
		set
		{
			_hasPendingDishes = value;
			VisualIndicator.gameObject.SetActive(value);
			_tableCollider.enabled = value;
		}

		get => _hasPendingDishes;
	}

	private void Awake()
	{
		_droneIsAtTable = false;
		_droneContainer = Drone.GetComponent<DroneContainer>();
		_tableCollider = GetComponent<Collider2D>();
		_tableCollider.enabled = false;
	}

	private void Update()
	{
		if (HasActiveOrder || HasPendingDishes)
		{
			UpdateIndicator();

			if (_droneIsAtTable && Drone.IsSleeping() && Vector3.Angle(Drone.transform.up, Vector3.up) <= 15f)
			{
				Debug.Log("At table!!");

				if (HasActiveOrder && _droneContainer.DeliverableList.Count != 0)
				{
					HasActiveOrder = false;

					var score = CalculateScore();

					Debug.Log(score);

					foreach (var item in _droneContainer.DeliverableList)
					{
						Destroy(item.gameObject);
					}

					_droneContainer.DeliverableList.Clear();

					OnOrderDelivered.Invoke(this);
				}

				if (HasPendingDishes && _droneContainer.DeliverableList.Count == 0)
				{
					HasPendingDishes = false;

					var dishesPrefab = Instantiate(_dishesPrefab, Drone.transform.position, Quaternion.identity);

					foreach (Transform child in dishesPrefab.transform)
					{
						_droneContainer.DeliverableList.Add(child.gameObject);
					}

					dishesPrefab.transform.DetachChildren();

					Destroy(dishesPrefab);

					OnDishesCollected.Invoke(this);
				}
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

	public void NewDishes(GameObject dishesPrefab)
	{
		_dishesPrefab = dishesPrefab;
		HasPendingDishes = true;
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

	public void Reset()
	{
		_droneIsAtTable = false;
		_tableCollider.enabled = false;
		HasActiveOrder = false;
		HasPendingDishes = false;

		OnOrderDelivered = null;
		OnDishesCollected = null;
	}
}
