using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[Tooltip("�÷��̾� �ȱ� �ӵ�")] public int anySpeed;
	[Tooltip("�÷��̾� �ȱ� �ӵ�")] public int runSpeed;
	public bool isAttack = false;	//�÷��̾ ���� ������ Ȯ��

	private new Rigidbody rigidbody;
	private PlayerAnimation anim;
	
	public void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		anim = GetComponent<PlayerAnimation>();
		if (GameManager.instance.reStart)
			transform.position = GameManager.instance.reStartHeroPosition;
		else
			transform.position = GameManager.instance.lastHeroPosition;
		
		GameManager.instance.FindEnemy();
		GameManager.instance.OffEnemy();
	}
	
	private void FixedUpdate()
	{
		if (!isAttack)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				//�÷��̾ �޸� ��
				DoMove(runSpeed,2);
			}
			else
			{
				//�÷��̾ ���� ��
				DoMove(anySpeed,1);
			}
		}

		if(Input.GetMouseButtonDown(0) && !isAttack)
		{
			StartCoroutine(DoAttack());
		}
	}

	//�÷��̾� �̵�
	private void DoMove(int _speed, int _runMotion)
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");
		Vector3 moveH = transform.right * h;
		Vector3 moveV = transform.forward * v;
		Vector3 velocity = (moveH + moveV).normalized * _speed;
		rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);
		anim.DoMoveAnim(h * _runMotion, v * _runMotion);
	}

	//�÷��̾� ����
	IEnumerator DoAttack()
	{
		anim.DoPlayerAttack();
		yield return new WaitForSeconds(0.02f);
		isAttack = true;
		float animTime = anim.GetAnimPlayTime();
		//Debug.Log(animTime + " " + isAttack);
		yield return new WaitForSeconds(animTime);
		isAttack = false;
	}

}
