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
	public void AttackCollTrue()   //애니메이션 이벤트 사용 (Mushroom_Attack01Smile)
	{
		playerAttackBox.SetActive(true);
	}
	public void AttackCollFalse()   //애니메이션 이벤트 사용 (Mushroom_Attack01Smile)
	{
		playerAttackBox.SetActive(false);
	}
}
