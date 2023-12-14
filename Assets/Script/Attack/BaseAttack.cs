using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
	public string attackName;//공격 이름
	public string attackDescription; //공격 설명
	public float attackDamage;//기본 데미지 15, 레벨 10 스테미나 35 = 기본데미지 + 스테미나 + 레벨
	public float attackCost;//마나 코스트

}
