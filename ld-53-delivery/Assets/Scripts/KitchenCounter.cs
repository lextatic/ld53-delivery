using System.Collections.Generic;
using UnityEngine;

public class KitchenCounter : MonoBehaviour
{
	public Rigidbody2D Drone;

	// Trocar por dados, ScriptableObject provavelmente.
	public GameObject DeliverablePrefab;

	public List<Table> Tables;

	public Transform VisualIndicator;

	private bool _droneIsAtCounter;
	private bool _hasPendingOrder;
	private bool _hasActiveDishes;
	private Collider2D _counterCollider;
	private DroneContainer _droneContainer;

	public bool HasPendingOrder
	{
		set
		{
			_hasPendingOrder = value;
			VisualIndicator.gameObject.SetActive(value);
			_counterCollider.enabled = value;
		}

		get => _hasPendingOrder;
	}

	public bool HasActiveDishes
	{
		set
		{
			_hasActiveDishes = value;
			VisualIndicator.gameObject.SetActive(value);
			_counterCollider.enabled = value;
		}

		get => _hasActiveDishes;
	}

	private void Awake()
	{
		_droneIsAtCounter = false;
		_droneContainer = Drone.GetComponent<DroneContainer>();
		_counterCollider = GetComponent<Collider2D>();
		_counterCollider.enabled = false;
		HasPendingOrder = true;
	}

	private void Update()
	{
		if (HasPendingOrder || HasActiveDishes)
		{
			UpdateIndicator();

			if (_droneIsAtCounter && Drone.IsSleeping())
			{
				Debug.Log("At counter!!");

				if (HasPendingOrder && _droneContainer.DeliverableList.Count == 0)
				{
					NewOrder();
				}

				if (HasActiveDishes && _droneContainer.DeliverableList.Count != 0)
				{
					HasActiveDishes = false;

					//var score = CalculateScore();

					Debug.Log("dishes delivered");

					foreach (var item in _droneContainer.DeliverableList)
					{
						Destroy(item.gameObject);
					}

					_droneContainer.DeliverableList.Clear();

					HasPendingOrder = true;
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == Drone.gameObject)
		{
			_droneIsAtCounter = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject == Drone.gameObject)
		{
			_droneIsAtCounter = false;
		}
	}

	private void NewOrder()
	{
		int table = Random.Range(0, Tables.Count);

		Tables[table].HasActiveOrder = true;
		Tables[table].OnOrderDelivered += KitchenCounter_OnOrderDelivered;
		HasPendingOrder = false;

		var deliverablePrefab = Instantiate(DeliverablePrefab, Drone.transform.position, Quaternion.identity);

		foreach (Transform child in deliverablePrefab.transform)
		{
			_droneContainer.DeliverableList.Add(child.gameObject);
		}

		deliverablePrefab.transform.DetachChildren();

		Destroy(deliverablePrefab);
	}

	private void KitchenCounter_OnOrderDelivered(Table table)
	{
		table.OnOrderDelivered -= KitchenCounter_OnOrderDelivered;

		NewDishes();
	}

	private void NewDishes()
	{
		int table = Random.Range(0, Tables.Count);

		Tables[table].NewDishes(DeliverablePrefab);
		Tables[table].OnDishesCollected += KitchenCounter_OnDishesCollected;
	}

	private void KitchenCounter_OnDishesCollected(Table table)
	{
		HasActiveDishes = true;
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
