using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
	public string attackName;//���� �̸�
	public string attackDescription; //���� ����
	public float attackDamage;//�⺻ ������ 15, ���� 10 ���׹̳� 35 = �⺻������ + ���׹̳� + ����
	public float attackCost;//���� �ڽ�Ʈ

}
