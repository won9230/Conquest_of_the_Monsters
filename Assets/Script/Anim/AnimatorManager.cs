using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
	public Animator anim;

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	public void RunAnim(bool _setBool)
	{
		anim.SetBool("Run", _setBool);
	}

	public void AttackAnim(bool _setBool)
	{
		anim.SetBool("Attack", _setBool);
	}

	public void DieAnim(bool _setBool)
	{
		anim.SetBool("Die", _setBool);
	}

	public void TakeDamageAnim()
	{
		anim.SetTrigger("TakeDamage");
	}
	
	//���� �������� �ִϸ��̼� �ð��� ��ȯ
	public float GetAnimTime()
	{
		return anim.GetCurrentAnimatorStateInfo(0).length;
	}
}