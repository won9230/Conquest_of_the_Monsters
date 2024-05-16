using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackColl : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			GameManager.instance.curRegions = other.GetComponent<EnemyAI>().monsterID;
			GameManager.instance.gamestate = GameManager.GameState.Battle_State;
			GameManager.instance.DeadEnemyListAdd(transform.parent.gameObject.name);
			//Debug.Log("������ ���ߴ�! " + this.gameObject.name);
			//Debug.Log($"battleEnemy�� {GameManager.instance.battleEnemy}�� ����Ǿ����ϴ�");
		}
	}
}
