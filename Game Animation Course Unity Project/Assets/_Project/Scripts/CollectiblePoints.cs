using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblePoints : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void GetPoints(int amount)
	{
		GameManager.Instance.GetPoints(amount);
	}
}
