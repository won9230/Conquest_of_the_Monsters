using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTrun 
{
	public string attacker;     //�����ϴ� ������Ʈ �̸�
	public string Type;
	public GameObject attackersGamgeObject;	//�����ϴ� ������Ʈ
	public GameObject attackersTarget;      //������ ������Ʈ
	public int ahility;	//���Ŀ� ����� ��ø

	public BaseAttack choosenAttack;
}
