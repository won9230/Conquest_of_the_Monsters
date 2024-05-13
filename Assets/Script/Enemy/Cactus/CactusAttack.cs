using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusAttack : MonoBehaviour
{
	public int enemyId = 0;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			//BattleCutControl.instance.BattleCutPlay();
			GameManager.instance.curRegions = 0;
			GameManager.instance.gamestate = GameManager.GameState.Battle_State;
			//Debug.Log("공격을 당했다! " + this.gameObject.name);
		}
	}
}
