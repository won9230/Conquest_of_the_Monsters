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
	public string attackerName;	//Find�� ã�� �̸�
	public int ahility; //���� ���� ��ø
	public HeroToEnemy heroToEnemy; //�������� ������ �ν�
}
