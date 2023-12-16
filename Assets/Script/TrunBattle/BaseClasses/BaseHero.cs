using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero : BaseClass
{
	public int stamina;		//Èû
	public int intellect;	//Áö·Â
	public int dexterity;	//ÀçÄ¡
	public int ahility;     //¹ÎÃ¸

	public List<BaseAttack> magicAttacks = new List<BaseAttack>();
}
