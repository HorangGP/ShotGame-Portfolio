using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Bullet : MonoBehaviourPun
{
	[SerializeField] private float bulletSpeed = 500f; // 총알 속도
	[SerializeField] private GameObject tracerPrefab; // 궤적 프리팹

    private Rigidbody rig;
	private int shooterID; // 발사자의 PhotonView ID 저장

	private void Awake()
	{
		if (TryGetComponent<Rigidbody>(out var rigComponent))
		{
			rig = rigComponent;
			rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 빠른 속도의 물체에 대한 충돌 감지
		}
	}

	private void Start()
	{
        // ForceMode.VelocityChange는 현재 속도를 무시하고 즉시 원하는 속도로 설정 → 총알처럼 순간 가속이 필요한 상황에 적합
        rig.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);

		if (PhotonNetwork.InRoom && photonView.IsMine)
		{
			// 총알 궤적 그리라고 rpc 호출
			photonView.RPC("RpcShowTracer", RpcTarget.All, transform.position, transform.position + transform.forward * 50f);
		}
		else
		{
			RpcShowTracer(transform.position, transform.position + transform.forward * 50f);
		}

		Destroy(gameObject, 2f);
	}

	// 콜리전 충돌
	private void OnCollisionEnter(Collision coll)
	{
		if (coll.gameObject.TryGetComponent<PhotonView>(out var targetPhotonView))
		{
			// 피격 대상이 존재하고, 자신이 쏜 총알이라면 충돌 무시
			if (targetPhotonView.ViewID == shooterID)
			{
				Debug.Log("자기 자신에게 맞는 총알 무시!");
				return;
			}
		}

		Debug.Log(coll.gameObject.name + " 충돌!");

		Destroy(gameObject);
	}

	/// <summary>
	/// 발사자 ID 세팅
	/// </summary>
	/// <param name="id"></param>
	public void SetShooter(int id)
	{
		shooterID = id;
	}


	/// <summary>
	/// 모든 클라이언트들에게 총알 궤적 표시
	/// </summary>
	/// <param name="startPos"></param>
	/// <param name="endPos"></param>
	[PunRPC]
	private void RpcShowTracer(Vector3 startPos,Vector3 endPos)
	{
		GameObject tracer = Instantiate(tracerPrefab, startPos, Quaternion.identity);

		tracer.GetComponent<BulletTracer>().ShowTracer(startPos, endPos);
	}
}
