using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
	public GameObject playerAttackBox;


	private void Start()
	{
		playerAttackBox.SetActive(false);
	}
	public void AttackCollTrue()   //�ִϸ��̼� �̺�Ʈ ��� (Mushroom_Attack01Smile)
	{
		playerAttackBox.SetActive(true);
	}
	public void AttackCollFalse()   //�ִϸ��̼� �̺�Ʈ ��� (Mushroom_Attack01Smile)
	{
		playerAttackBox.SetActive(false);
	}
}
