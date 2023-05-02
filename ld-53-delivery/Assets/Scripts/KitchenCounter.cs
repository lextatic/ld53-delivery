using System;
using System.Collections.Generic;
using TMPro;
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

	public Score Score;

	public AudioSource MainAudioSource;
	public SimpleAudioEvent GetDishesAudio;
	public TextMeshProUGUI ProgressLabel;

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

		VisualIndicator.transform.parent = null;

		NewMission();
	}

	private void Update()
	{
		if (HasPendingOrder || HasActiveDishes)
		{
			UpdateIndicator();

			if (_droneIsAtCounter && (Drone.IsSleeping() || (Drone.velocity.sqrMagnitude < 0.1f && Drone.angularVelocity < 0.1f))
				&& Vector3.Angle(Drone.transform.up, Vector3.up) <= 15f)
			{
				if (HasPendingOrder && _droneContainer.DeliverableList.Count == 0)
				{
					CreateOrder();
				}

				if (HasActiveDishes && _droneContainer.DeliverableList.Count != 0)
				{
					Score.CurrentScore += CalculateScoreLoss();

					HasActiveDishes = false;

					foreach (var item in _droneContainer.DeliverableList)
					{
						Destroy(item.gameObject);
					}

					_droneContainer.DeliverableList.Clear();

					GetDishesAudio.Play(MainAudioSource);

					NewMission();
				}
			}
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
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

		GetDishesAudio.Play(MainAudioSource);

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
		Vector3 objectPosition = transform.position;
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(objectPosition);

		var visualIndicatorPosition = objectPosition;
		var visualIndicatorRotation = transform.rotation;

		if (screenPosition.x < 0 || screenPosition.x > Screen.width || screenPosition.y < 0 || screenPosition.y > Screen.height)
		{
			Vector3 cameraToObjectDirection = (objectPosition - Drone.transform.position).normalized;
			Vector3 edgePosition = Drone.transform.position + (cameraToObjectDirection * 20);
			Vector3 edgeScreenPosition = Camera.main.WorldToScreenPoint(edgePosition);
			visualIndicatorPosition = Camera.main.ScreenToWorldPoint(edgeScreenPosition);

			Vector3 direction = (visualIndicatorPosition - objectPosition).normalized;
			float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
			visualIndicatorRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}

		VisualIndicator.transform.position = Vector3.Lerp(VisualIndicator.transform.position, visualIndicatorPosition, 2 * Time.deltaTime);
		VisualIndicator.transform.rotation = Quaternion.Lerp(VisualIndicator.transform.rotation, visualIndicatorRotation, 2 * Time.deltaTime);
	}

	private int CalculateScoreLoss()
	{
		int totalScore = 0;

		foreach (var item in _droneContainer.DeliverableList)
		{
			if (!Physics2D.Raycast(item.transform.position + (Vector3.up * 0.5f), Vector2.down, 5.5f, 1 << LayerMask.NameToLayer("Score")))
			{
				totalScore += item.GetComponent<DeliverableScore>().ReadScore();
			}
		}

		return totalScore;
	}

	private void NewMission()
	{
		ProgressLabel.text = $"<color=#FFB100>Progress:</color> {_currentMissionIndex + 1}/{Missions.Count}";

		if (_currentMissionIndex >= Missions.Count)
		{
			Drone.GetComponent<Controller>().ShowVictoryPanel();
			return;
		}

		SaveCheckpoint();

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

	private Vector3 _savedPosition;
	private int _savedScore;
	private int _savedMissionIndex;

	private void SaveCheckpoint()
	{
		_savedPosition = Drone.position;
		_savedMissionIndex = _currentMissionIndex;
	}

	public void LoadCheckpoint()
	{
		_currentMissionIndex = _savedMissionIndex;

		HasPendingOrder = false;
		HasActiveDishes = false;
		_droneIsAtCounter = false;
		_counterCollider.enabled = false;

		foreach (var item in _droneContainer.DeliverableList)
		{
			Destroy(item.gameObject);
		}

		_droneContainer.DeliverableList.Clear();

		foreach (var table in Tables)
		{
			table.Reset();
		}

		Drone.velocity = Vector2.zero;
		Drone.angularVelocity = 0;
		Drone.isKinematic = true;
		Drone.position = _savedPosition;
		Drone.rotation = 0;
		Drone.isKinematic = false;

		NewMission();
	}
}
