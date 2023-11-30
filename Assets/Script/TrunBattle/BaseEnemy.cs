using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy
{
	public string name;

	public enum Type
	{
		GRASS,
		FIRE,
		WATER,
		ELECTRIC
	}

	public Type EnemyType;

	public float baseHp;
	public float curHp;

	public float baseMp;
	public float curMp;

	public float baseATK;
	public float curATK;
	public float baseDEF;
	public float curDEF;

}
