using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/* 플레이어의 상태 스크립트 
 * 플레이어 생존, 피격, 사망 상태를 나타내기 위함.
 */

public class PlayerState : MonoBehaviourPun
{
	/// <summary>
	/// 플레이어가 사망했을 때 호출할 메서드를 저장
	/// </summary>
	/// <param name="player"></param>
	public delegate void PlayerDieDelegate(GameObject player);

	/// <summary>
	/// 플레이어가 사망할 때 호출, 구독한 메서드들이 실행
	/// </summary>	
	public event PlayerDieDelegate OnPlayerDied;

	private bool isHit; // 피격
	private bool isDie; // 사망

	// 프로퍼티 간략화 - Get문을 간략화
	public bool IsHit => isHit;
	public bool IsDie => isDie;

	private void Start()
	{
		isHit = false; // 초기값은 피격되지 않음
		isDie = false; // 초기값은 사망하지 않음

		// 레그돌 효과 비활성화
		DisableRagdoll();
	}

	private void OnCollisionEnter(Collision coll)
	{
		if (coll.collider.CompareTag("Bullet") && !isHit)
		{
			Debug.Log("피격!");

			Vector3 hitForce = coll.relativeVelocity * coll.rigidbody.mass; // 충격 힘

			Vector3 hitPoint = coll.contacts[0].point; // 충격 지점

			OnHit(hitForce, hitPoint);
		}
	}

	public void OnHit(Vector3 hitForce, Vector3 hitPoint)
	{
		if (!isHit)
		{
			isHit = true; // 피격 상태로 변경

			EnableRagdoll(hitForce, hitPoint); // 레그돌 효과 활성화

			OnDie();
		}
	}

	/// <summary>
	/// 플레이어 사망처리
	/// </summary>
	private void OnDie()
	{
		if (!isDie)
		{
			isDie = true;

			// 사망 처리 후, 이벤트 발생 / **?.Invoke() : 구독자가 있을 경우에만 호출
			OnPlayerDied?.Invoke(this.gameObject);

			GameManager.Instance.IsGamePlaying = false; // 게임 종료
		}
	}


	/// <summary>
	/// 레그돌 효과 활성화
	/// </summary>
	/// <param name="hitForce">충격 힘</param>
	/// <param name="hitPoint">충격 지점</param>
	private void EnableRagdoll(Vector3 hitForce, Vector3 hitPoint)
	{
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		Collider[] colliders = GetComponentsInChildren<Collider>();

		// 모든 Rigidbody의 isKinematic을 false로 설정
		foreach (Rigidbody rb in rigidbodies)
		{
			if (rb != this.GetComponent<Rigidbody>())
			{
				// 자신의 Rigidbody는 제외
				// 물리 엔진의 영향을 받도록 설정
				rb.isKinematic = false;
			}
			else
			{
				// 자신의 Rigidbody는 물리 엔진의 영향을 받지 않도록 설정
				rb.isKinematic = true;
			}
		}

		// 모든 Collider의 enabled를 true로 설정
		foreach (Collider col in colliders)
		{
			if (col != this.GetComponent<Collider>())
			{
				// 자신의 Collider는 제외
				// 물리 엔진의 영향을 받도록 설정
				col.enabled = true;
			}
			else
			{
				// 자신의 Collider는 물리 엔진의 영향을 받지 않도록 설정
				col.enabled = true;
			}
		}

		if (TryGetComponent<Animator>(out var animator))
		{
			// Ragdoll 효과를 방해할 모든 애니메이션 효과를 비활성화
			animator.enabled = false;
		}

		// 충격 방향으로 힘을 가하여 플레이어가 자연스럽게 쓰러지도록 함.
		if (rigidbodies.Length > 0)
		{
			// 충격을 가할 Rigidbody를 선택 (여기서는 첫 번째 Rigidbody를 사용)
			Rigidbody rb = rigidbodies[0];

			// 충격 지점(hitPoint)에서 충격 힘(hitForce)을 가합니다.
			rb.AddForceAtPosition(hitForce, hitPoint, ForceMode.Impulse);

			// *AddForceAtPosition: 특정 지점에서 힘을 가하여 물리적 반응을 유도
			// *ForceMode.Impulse: 순간적인 힘을 가하는 모드
		}
	}

	/// <summary>
	/// 평상시 레그돌 효과 비활성화
	/// </summary>
	void DisableRagdoll()
	{
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		Collider[] colliders = GetComponentsInChildren<Collider>();

		// 모든 Rigidbody의 isKinematic을 true로 설정
		foreach (Rigidbody rb in rigidbodies)
		{
			if (rb != this.GetComponent<Rigidbody>())
			{
				// 자신의 Rigidbody는 제외
				// 물리 엔진의 영향을 받지 않도록 설정
				rb.isKinematic = true;
			}
		}
		// 모든 Collider의 enabled를 false로 설정
		foreach (Collider col in colliders)
		{
			if (col != this.GetComponent<Collider>())
			{
				// 자신의 Collider는 제외
				// 물리 엔진의 영향을 받지 않도록 설정
				col.enabled = false;
			}
		}
		if (TryGetComponent<Animator>(out var animator))
		{
			animator.enabled = true;
		}
	}
}
