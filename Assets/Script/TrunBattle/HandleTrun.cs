using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTrun 
{
	public string attacker;     //공격하는 오브젝트 이름
	public string Type;
	public GameObject attackersGamgeObject;	//공격하는 오브젝트
	public GameObject attackersTarget;      //공격할 오브젝트
	public int ahility;	//정렬에 사용할 민첩

	public BaseAttack choosenAttack;
}
