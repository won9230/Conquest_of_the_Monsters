using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEevnt : MonoBehaviour
{
	public GameObject attackColl;
	private void Start()
	{
		attackColl.SetActive(false);
	}
	public void AttackCollTrue()   //�ִϸ��̼� �̺�Ʈ ��� (Mushroom_Attack01Smile)
	{
		attackColl.SetActive(true);
	}
	public void AttackCollFalse()   //�ִϸ��̼� �̺�Ʈ ��� (Mushroom_Attack01Smile)
	{
		attackColl.SetActive(false);
	}
}
