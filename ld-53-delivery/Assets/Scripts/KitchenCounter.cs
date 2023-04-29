using UnityEngine;

public class KitchenCounter : MonoBehaviour
{
	public Rigidbody2D Drone;
	public GameObject DeliverablePrefab;

	private bool _droneIsAtCounter;
	private DroneContainer _droneContainer;

	private void Awake()
	{
		_droneIsAtCounter = false;
		_droneContainer = Drone.GetComponent<DroneContainer>();
	}

	private void Update()
	{
		if (_droneIsAtCounter && Drone.IsSleeping() && _droneContainer.DeliverableList.Count == 0)
		{
			Debug.Log("At counter!!");

			var deliverablePrefab = Instantiate(DeliverablePrefab, Drone.transform.position, Quaternion.identity);

			foreach (Transform child in deliverablePrefab.transform)
			{
				_droneContainer.DeliverableList.Add(child.gameObject);
			}

			deliverablePrefab.transform.DetachChildren();

			Destroy(deliverablePrefab);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == Drone.gameObject)
		{
			Debug.Log("On the counter");

			_droneIsAtCounter = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject == Drone.gameObject)
		{
			Debug.Log("Off the counter");

			_droneIsAtCounter = false;
		}
	}
}
