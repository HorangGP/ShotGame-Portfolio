using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/* �÷��̾��� ���� ��ũ��Ʈ 
 * �÷��̾� ����, �ǰ�, ��� ���¸� ��Ÿ���� ����.
 */

public class PlayerState : MonoBehaviourPun
{
	/// <summary>
	/// �÷��̾ ������� �� ȣ���� �޼��带 ����
	/// </summary>
	/// <param name="player"></param>
	public delegate void PlayerDieDelegate(GameObject player);

	/// <summary>
	/// �÷��̾ ����� �� ȣ��, ������ �޼������ ����
	/// </summary>	
	public event PlayerDieDelegate OnPlayerDied;

	private bool isHit; // �ǰ�
	private bool isDie; // ���

	// ������Ƽ ����ȭ - Get���� ����ȭ
	public bool IsHit => isHit;
	public bool IsDie => isDie;

	private void Start()
	{
		isHit = false; // �ʱⰪ�� �ǰݵ��� ����
		isDie = false; // �ʱⰪ�� ������� ����

		// ���׵� ȿ�� ��Ȱ��ȭ
		DisableRagdoll();
	}

	private void OnCollisionEnter(Collision coll)
	{
		if (coll.collider.CompareTag("Bullet") && !isHit)
		{
			Debug.Log("�ǰ�!");

			Vector3 hitForce = coll.relativeVelocity * coll.rigidbody.mass; // ��� ��

			Vector3 hitPoint = coll.contacts[0].point; // ��� ����

			OnHit(hitForce, hitPoint);
		}
	}

	public void OnHit(Vector3 hitForce, Vector3 hitPoint)
	{
		if (!isHit)
		{
			isHit = true; // �ǰ� ���·� ����

			EnableRagdoll(hitForce, hitPoint); // ���׵� ȿ�� Ȱ��ȭ

			OnDie();
		}
	}

	/// <summary>
	/// �÷��̾� ���ó��
	/// </summary>
	private void OnDie()
	{
		if (!isDie)
		{
			isDie = true;

			// ��� ó�� ��, �̺�Ʈ �߻� / **?.Invoke() : �����ڰ� ���� ��쿡�� ȣ��
			OnPlayerDied?.Invoke(this.gameObject);

			GameManager.Instance.IsGamePlaying = false; // ���� ����
		}
	}


	/// <summary>
	/// ���׵� ȿ�� Ȱ��ȭ
	/// </summary>
	/// <param name="hitForce">��� ��</param>
	/// <param name="hitPoint">��� ����</param>
	private void EnableRagdoll(Vector3 hitForce, Vector3 hitPoint)
	{
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		Collider[] colliders = GetComponentsInChildren<Collider>();

		// ��� Rigidbody�� isKinematic�� false�� ����
		foreach (Rigidbody rb in rigidbodies)
		{
			if (rb != this.GetComponent<Rigidbody>())
			{
				// �ڽ��� Rigidbody�� ����
				// ���� ������ ������ �޵��� ����
				rb.isKinematic = false;
			}
			else
			{
				// �ڽ��� Rigidbody�� ���� ������ ������ ���� �ʵ��� ����
				rb.isKinematic = true;
			}
		}

		// ��� Collider�� enabled�� true�� ����
		foreach (Collider col in colliders)
		{
			if (col != this.GetComponent<Collider>())
			{
				// �ڽ��� Collider�� ����
				// ���� ������ ������ �޵��� ����
				col.enabled = true;
			}
			else
			{
				// �ڽ��� Collider�� ���� ������ ������ ���� �ʵ��� ����
				col.enabled = true;
			}
		}

		if (TryGetComponent<Animator>(out var animator))
		{
			// Ragdoll ȿ���� ������ ��� �ִϸ��̼� ȿ���� ��Ȱ��ȭ
			animator.enabled = false;
		}

		// ��� �������� ���� ���Ͽ� �÷��̾ �ڿ������� ���������� ��.
		if (rigidbodies.Length > 0)
		{
			// ����� ���� Rigidbody�� ���� (���⼭�� ù ��° Rigidbody�� ���)
			Rigidbody rb = rigidbodies[0];

			// ��� ����(hitPoint)���� ��� ��(hitForce)�� ���մϴ�.
			rb.AddForceAtPosition(hitForce, hitPoint, ForceMode.Impulse);

			// *AddForceAtPosition: Ư�� �������� ���� ���Ͽ� ������ ������ ����
			// *ForceMode.Impulse: �������� ���� ���ϴ� ���
		}
	}

	/// <summary>
	/// ���� ���׵� ȿ�� ��Ȱ��ȭ
	/// </summary>
	void DisableRagdoll()
	{
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		Collider[] colliders = GetComponentsInChildren<Collider>();

		// ��� Rigidbody�� isKinematic�� true�� ����
		foreach (Rigidbody rb in rigidbodies)
		{
			if (rb != this.GetComponent<Rigidbody>())
			{
				// �ڽ��� Rigidbody�� ����
				// ���� ������ ������ ���� �ʵ��� ����
				rb.isKinematic = true;
			}
		}
		// ��� Collider�� enabled�� false�� ����
		foreach (Collider col in colliders)
		{
			if (col != this.GetComponent<Collider>())
			{
				// �ڽ��� Collider�� ����
				// ���� ������ ������ ���� �ʵ��� ����
				col.enabled = false;
			}
		}
		if (TryGetComponent<Animator>(out var animator))
		{
			animator.enabled = true;
		}
	}
}
