using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero : BaseClass
{
	[Tooltip("��")]
	public int stamina;     //��
	[Tooltip("����")]
	public int intellect;	//����
	[Tooltip("����")] 
	public int dexterity;   //��ġ
	[Tooltip("��ø")]
	public int ahility;     //��ø

	public List<BaseAttack> magicAttacks = new List<BaseAttack>();
}
