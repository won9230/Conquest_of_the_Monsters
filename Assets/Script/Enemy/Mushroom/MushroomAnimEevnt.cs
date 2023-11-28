using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomAnimEevnt : MonoBehaviour
{
	public GameObject attackColl;
	private void Start()
	{
		attackColl.SetActive(false);
	}
	public void AttackCollTrue()   //애니메이션 이벤트 사용 (Mushroom_Attack01Smile)
	{
		attackColl.SetActive(true);
	}
	public void AttackCollFalse()   //애니메이션 이벤트 사용 (Mushroom_Attack01Smile)
	{
		attackColl.SetActive(false);
	}
}
