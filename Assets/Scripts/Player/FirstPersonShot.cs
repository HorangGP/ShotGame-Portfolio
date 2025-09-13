using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/* 1인칭 슈팅 스크립트 */

public class FirstPersonShot : MonoBehaviourPun
{
	[SerializeField] private Transform shootPos;
	[SerializeField] private GameObject bulletPrefab;
	private GunEffect gunEffect;

	private bool canShoot = false;

    // 레이 관련
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
        ray = new Ray(); // 레이 생성
		canShoot = false; // 초기값은 발사 불가
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
	/// 총알 발사
	/// </summary>
	public void Shooting()
	{
        if (!canShoot) return;

		Debug.Log("Shot!");
        //DrawingRayTest();

        if (PhotonNetwork.InRoom)
		{
            // 총알 생성
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
    /// 슈팅 가능 여부
    /// </summary>
    /// <param name="state"></param>
    public void EnableShoot(bool state)
    {
		canShoot = state;
    }

	//-------------------------------------------------------------

	/// <summary>
	/// 레이 설정
	/// </summary>
	void RaySetting()
	{
		ray.origin = shootPos.position; // 레이를 사용할 위치 지정
		ray.direction = shootPos.forward; // 레이 방향 지정

	}

    // 레이 그리기
	private void OnDrawGizmos()
    {
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.green);
    }

    // 레이 충돌 테스트
    void DrawingRayTest()
	{
		Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);

		if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 15.0f, layerMask))
		{
			hitInfo.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
		}
	}
}
