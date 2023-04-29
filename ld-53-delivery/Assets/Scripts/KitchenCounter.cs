using System.Collections.Generic;
using UnityEngine;

public class KitchenCounter : MonoBehaviour
{
	public Rigidbody2D Drone;

	// Trocar por dados, ScriptableObject provavelmente.
	public GameObject DeliverablePrefab;

	public List<Table> Tables;

	private bool _droneIsAtCounter;
	private bool _hasActiveOrder;
	private DroneContainer _droneContainer;

	private void Awake()
	{
		_droneIsAtCounter = false;
		_hasActiveOrder = false;
		_droneContainer = Drone.GetComponent<DroneContainer>();

		//for (int i = 0; i < Tables.Count; i++)
		//{
		//	Tables[0].TableID = i + 1;
		//}
	}

	private void Update()
	{
		if (_droneIsAtCounter && Drone.IsSleeping() && _droneContainer.DeliverableList.Count == 0 && !_hasActiveOrder)
		{
			Debug.Log("At counter!!");

			NewOrder();
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

		//Tables[table].HasActiveOrder = true;
		Tables[0].HasActiveOrder = true;
		Tables[0].OnOrderDelivered += KitchenCounter_OnOrderDelivered;
		_hasActiveOrder = true;

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
		_hasActiveOrder = false;
		table.OnOrderDelivered -= KitchenCounter_OnOrderDelivered;
	}
}
