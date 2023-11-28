using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomAttack : MonoBehaviour
{

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			BattleCutControl.instance.BattleCutPlay();
			Debug.Log("공격을 당했다!");
		}
	}
}
