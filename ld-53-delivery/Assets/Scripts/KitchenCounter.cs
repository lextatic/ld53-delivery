using System;
using System.Collections.Generic;
using UnityEngine;

public enum MissionType
{
	Order,
	Dishes
}

[Serializable]
public struct Mission
{
	public int TableID;
	public MissionType MissionType;
	public GameObject DeliverableComboPrefab;
}

public class KitchenCounter : MonoBehaviour
{
	public Rigidbody2D Drone;
	public Transform VisualIndicator;

	public List<Table> Tables;
	public List<Mission> Missions;

	private bool _droneIsAtCounter;
	private bool _hasPendingOrder;
	private bool _hasActiveDishes;
	private Collider2D _counterCollider;
	private DroneContainer _droneContainer;
	private int _currentMissionIndex;

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

	private void Start()
	{
		_droneIsAtCounter = false;
		_droneContainer = Drone.GetComponent<DroneContainer>();
		_counterCollider = GetComponent<Collider2D>();
		_counterCollider.enabled = false;
		_currentMissionIndex = 0;

		NewMission();
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
					CreateOrder();
				}

				if (HasActiveDishes && _droneContainer.DeliverableList.Count != 0)
				{
					HasActiveDishes = false;

					var score = CalculateScoreLoss();

					Debug.Log(score);

					foreach (var item in _droneContainer.DeliverableList)
					{
						Destroy(item.gameObject);
					}

					_droneContainer.DeliverableList.Clear();

					NewMission();
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

	private void CreateOrder()
	{
		var mission = Missions[_currentMissionIndex];

		Tables[mission.TableID].HasActiveOrder = true;
		Tables[mission.TableID].OnOrderDelivered += KitchenCounter_OnOrderDelivered;
		HasPendingOrder = false;
		_currentMissionIndex++;

		var deliverablePrefab = Instantiate(mission.DeliverableComboPrefab, Drone.transform.position, Quaternion.identity);

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

		NewMission();
	}

	private void NewDishes(int tableID, GameObject deliverableComboPrefab)
	{
		Tables[tableID].NewDishes(deliverableComboPrefab);
		Tables[tableID].OnDishesCollected += KitchenCounter_OnDishesCollected;
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

	private int CalculateScoreLoss()
	{
		int totalScore = 0;

		foreach (var item in _droneContainer.DeliverableList)
		{
			if (!Physics2D.Raycast(item.transform.position, Vector2.down, 5f, 1 << LayerMask.NameToLayer("Score")))
			{
				totalScore -= item.GetComponent<DeliverableScore>().Score;
			}
		}

		return totalScore;
	}

	private void NewMission()
	{
		if (_currentMissionIndex >= Missions.Count)
		{
			Debug.Log("Victory!");
			return;
		}

		var mission = Missions[_currentMissionIndex];

		switch (mission.MissionType)
		{
			case MissionType.Order:
				HasPendingOrder = true;
				break;

			case MissionType.Dishes:
				NewDishes(mission.TableID, mission.DeliverableComboPrefab);
				_currentMissionIndex++;
				break;
		}
	}
}
