using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/* 상체 상하 이동 스크립트 */

public class UpperBodyRotator : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Transform upperBody; // 로컬 상체 본
	[SerializeField] private Camera playerCamera; // 로컬 플레이어 카메라

	private float syncedXRot = 0f; // 다른 플레이어로부터 받은 상체 회전 값
	private float currentXRot = 0f; // 실제 적용할 회전값



	// 회전값을 적용하는 타이밍은 LateUpdate로 설정 (카메라 회전 후 적용하기 위함)
	private void LateUpdate()
    {
		if (photonView.IsMine)
		{
			// 로컬 플레이어의 상체 회전값을 가져옴.
			// ★ localEulerAngles.x 는 카메라가 위, 아래로 얼마나 기울었는지 알려주는 값.
			float xRot = playerCamera.transform.localEulerAngles.x;

			// 회전 각도는 0 ~ 360 사이의 값으로 표현되므로, 180도를 넘어가면 -360도를 빼서 -180도 ~ 180도 범위로 변환
			if (xRot > 180f)
			{
                xRot -= 360f;
			}

			// 상체 오브젝트를 xRot만큼 회전
            upperBody.localRotation = Quaternion.Euler(xRot, 0f, 0f);
		}
		else
		{
			// 다른 플레이어에게 받은 회전 (Lerp 보간 적용)
			currentXRot = Mathf.Lerp(currentXRot, syncedXRot, Time.deltaTime * 10f);

			// 상대의 상체 오브젝트를 받은 회전값만큼 회전
			upperBody.localRotation = Quaternion.Euler(currentXRot, 0f, 0f);
		}
	}



	/// <summary>
	/// PUN은 해당 메서드를 통해 내 오브젝트의 상태를 전송하거나, 다른 오브젝트의 상태를 수신.
	/// </summary>
	/// <param name="stream">전송 또는 수신에 사용되는 스트림(통로)</param>
	/// <param name="info">메세지의 부가정보(보낸 시간 등)</param>
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		// 해당 오브젝트가 자신일 때
		if (stream.IsWriting)
		{
			float xRot = playerCamera.transform.localEulerAngles.x;

			if (xRot > 180f)
			{
				xRot -= 360f;
			}

			// 로컬 플레이어(나)의 상체 회전값을 상대방에게 전송
			stream.SendNext(xRot); 

		}
		else
		{
			// 다른 플레이어의 상체 회전값을 수신받아 적용
			syncedXRot = (float)stream.ReceiveNext(); 
		}
	}
}

/*
 * ★ IPunObservable 인터페이스는 Photon 네트워크에서 객체의 상태를 지속적으로 송수신하기 위해 사용.
 * 
 * ★ PhotonView 내 Observervables의 Synchronization 설정 ★
 * 
 * - Unreliable On Change : 상태가 변경될 때만 전송 (기본값), 빠르게 자주 바뀌는 값엔 부적합.
 * 
 * - Unreliable : 항상 전송 (매 프레임), 빠르지만 값이 손실될 수 있음.
 * 
 * - Reliable Delta Compressed : 상태가 변경될 때만 변경된 값만 전송 (압축), 자주 바뀌는 값에 적합.
 */