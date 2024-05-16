using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollEevnt : MonoBehaviour
{

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			//BattleCutControl.instance.BattleCutPlay();
			GameManager.instance.curRegions = GetComponentInParent<EnemyAI>().monsterID;
			GameManager.instance.gamestate = GameManager.GameState.Battle_State;
			GameManager.instance.DeadEnemyListAdd(transform.parent.gameObject.name);
			//Debug.Log("공격을 당했다! " + this.gameObject.name);
			//Debug.Log($"battleEnemy에 {GameManager.instance.battleEnemy}가 저장되었습니다");
		}
	}
}
