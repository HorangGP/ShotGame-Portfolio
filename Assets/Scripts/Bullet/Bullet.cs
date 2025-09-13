using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Bullet : MonoBehaviourPun
{
	[SerializeField] private float bulletSpeed = 500f; // �Ѿ� �ӵ�
	[SerializeField] private GameObject tracerPrefab; // ���� ������

    private Rigidbody rig;
	private int shooterID; // �߻����� PhotonView ID ����

	private void Awake()
	{
		if (TryGetComponent<Rigidbody>(out var rigComponent))
		{
			rig = rigComponent;
			rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // ���� �ӵ��� ��ü�� ���� �浹 ����
		}
	}

	private void Start()
	{
        // ForceMode.VelocityChange�� ���� �ӵ��� �����ϰ� ��� ���ϴ� �ӵ��� ���� �� �Ѿ�ó�� ���� ������ �ʿ��� ��Ȳ�� ����
        rig.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);

		if (PhotonNetwork.InRoom && photonView.IsMine)
		{
			// �Ѿ� ���� �׸���� rpc ȣ��
			photonView.RPC("RpcShowTracer", RpcTarget.All, transform.position, transform.position + transform.forward * 50f);
		}
		else
		{
			RpcShowTracer(transform.position, transform.position + transform.forward * 50f);
		}

		Destroy(gameObject, 2f);
	}

	// �ݸ��� �浹
	private void OnCollisionEnter(Collision coll)
	{
		if (coll.gameObject.TryGetComponent<PhotonView>(out var targetPhotonView))
		{
			// �ǰ� ����� �����ϰ�, �ڽ��� �� �Ѿ��̶�� �浹 ����
			if (targetPhotonView.ViewID == shooterID)
			{
				Debug.Log("�ڱ� �ڽſ��� �´� �Ѿ� ����!");
				return;
			}
		}

		Debug.Log(coll.gameObject.name + " �浹!");

		Destroy(gameObject);
	}

	/// <summary>
	/// �߻��� ID ����
	/// </summary>
	/// <param name="id"></param>
	public void SetShooter(int id)
	{
		shooterID = id;
	}


	/// <summary>
	/// ��� Ŭ���̾�Ʈ�鿡�� �Ѿ� ���� ǥ��
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
