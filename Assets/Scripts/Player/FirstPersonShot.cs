using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/* 1��Ī ���� ��ũ��Ʈ */

public class FirstPersonShot : MonoBehaviourPun
{
	[SerializeField] private Transform shootPos;
	[SerializeField] private GameObject bulletPrefab;
	private GunEffect gunEffect;

	private bool canShoot = false;

    // ���� ����
    private float distance = 5f;
    private Ray ray;
    private RaycastHit hitInfo;
	private LayerMask layerMask;

	private void Awake()
	{
		if (TryGetComponent<GunEffect>(out var gunEffectComponent))
		{
			gunEffect = gunEffectComponent;
		}
	}

	// Start is called before the first frame update
	private void Start()
    {
        ray = new Ray(); // ���� ����
		canShoot = false; // �ʱⰪ�� �߻� �Ұ�
	}

	// Update is called once per frame
	private void Update()
    {
		if (photonView.IsMine && canShoot)
		{
            RaySetting();
        }
    }

	/// <summary>
	/// �Ѿ� �߻�
	/// </summary>
	public void Shooting()
	{
        if (!canShoot) return;

		Debug.Log("Shot!");
        //DrawingRayTest();

        if (PhotonNetwork.InRoom)
		{
            // �Ѿ� ����
            PhotonNetwork.Instantiate(bulletPrefab.name, shootPos.position, shootPos.rotation);
			photonView.RPC("OtherPlayEffect", RpcTarget.All);
        }
        else
		{
            Instantiate(bulletPrefab, shootPos.position, shootPos.rotation);
            gunEffect?.SinglePlayEffect();
        }

    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /// <param name="state"></param>
    public void EnableShoot(bool state)
    {
		canShoot = state;
    }

	//-------------------------------------------------------------

	/// <summary>
	/// ���� ����
	/// </summary>
	void RaySetting()
	{
		ray.origin = shootPos.position; // ���̸� ����� ��ġ ����
		ray.direction = shootPos.forward; // ���� ���� ����

	}

    // ���� �׸���
	private void OnDrawGizmos()
    {
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.green);
    }

    // ���� �浹 �׽�Ʈ
    void DrawingRayTest()
	{
		Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);

		if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 15.0f, layerMask))
		{
			hitInfo.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
		}
	}
}
