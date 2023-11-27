using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomAttack : MonoBehaviour
{
	public GameObject attackColl;

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			BattleCutControl.instance.BattleCutPlay();
		}
	}
}
