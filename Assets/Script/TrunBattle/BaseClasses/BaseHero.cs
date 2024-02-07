using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero : BaseClass
{
	[Tooltip("Èû")]
	public int stamina;     //Èû
	[Tooltip("Áö·Â")]
	public int intellect;	//Áö·Â
	[Tooltip("Áö·Â")] 
	public int dexterity;   //ÀçÄ¡
	[Tooltip("¹ÎÃ¸")]
	public int ahility;     //¹ÎÃ¸

	public List<BaseAttack> magicAttacks = new List<BaseAttack>();
}
