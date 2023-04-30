using System;
using UnityEngine;

public class Table : MonoBehaviour
{
	public int TableID;
	public Rigidbody2D Drone;
	public Transform VisualIndicator;
	public Score Score;

	public AudioSource MainAudioSource;
	public SimpleAudioEvent CashInAudio;
	public SimpleAudioEvent GetDishesAudio;

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

		VisualIndicator.transform.parent = null;
	}

	private void Update()
	{
		if (HasActiveOrder || HasPendingDishes)
		{
			UpdateIndicator();

			if (_droneIsAtTable && (Drone.IsSleeping() || (Drone.velocity.sqrMagnitude < 0.1f && Drone.angularVelocity < 0.1f))
				&& Vector3.Angle(Drone.transform.up, Vector3.up) <= 15f)
			{
				if (HasActiveOrder && _droneContainer.DeliverableList.Count != 0)
				{
					var score = CalculateScore();

					Score.CurrentScore += score;

					if (score > 0)
					{
						CashInAudio.Play(MainAudioSource);
					}

					HasActiveOrder = false;

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

					GetDishesAudio.Play(MainAudioSource);

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
			if (Physics2D.Raycast(item.transform.position + (Vector3.up * 0.5f), Vector2.down, 5.5f, 1 << LayerMask.NameToLayer("Score")))
			{
				if (Vector3.Angle(item.transform.up, Vector3.up) <= 15f)
				{
					totalScore += item.GetComponent<DeliverableScore>().ReadScore();
				}
			}
		}

		return totalScore;
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
