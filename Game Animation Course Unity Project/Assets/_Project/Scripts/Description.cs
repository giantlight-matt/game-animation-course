using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Description : MonoBehaviour
{
	public string description;

	public void ShowDescription()
	{
		GameManager.Instance.ShowDescription(description);
	}

	public void HideDescription()
	{
		GameManager.Instance.HideDescription();
	}
}
