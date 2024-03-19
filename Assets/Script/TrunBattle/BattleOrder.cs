using System.Collections;
using System.Collections.Generic;

public enum HeroToEnemy
{
	Hero,
	Enemy
}

[System.Serializable]
public class BattleOrder
{
	public string attackerName;	//Find로 찾을 이름
	public int ahility; //순서 정할 민첩
	public HeroToEnemy heroToEnemy; //영웅인지 적인지 인식
}
