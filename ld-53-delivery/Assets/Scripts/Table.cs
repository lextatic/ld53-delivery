using UnityEngine;

public class Table : MonoBehaviour
{
	public Rigidbody2D Drone;

	private bool _droneIsAtTable;
	private DroneContainer _droneContainer;

	private void Awake()
	{
		_droneIsAtTable = false;
		_droneContainer = Drone.GetComponent<DroneContainer>();
	}

	private void Update()
	{
		if (_droneIsAtTable && Drone.IsSleeping() && _droneContainer.DeliverableList.Count != 0)
		{
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
			Debug.Log("On the table");

			_droneIsAtTable = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject == Drone.gameObject)
		{
			Debug.Log("Off the able");

			_droneIsAtTable = false;
		}
	}
}
