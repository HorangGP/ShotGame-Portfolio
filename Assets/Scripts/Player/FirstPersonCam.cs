using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 1인칭 카메라 회전


// x축 회전 = 상하 회전
// y축 회전 = 좌우 회전

public class FirstPersonCam : MonoBehaviourPun
{
	/// <summary>
	/// 로컬 플레이어 바디
	/// </summary>
	private Transform playerBody;

	//[SerializeField] private float turnSpeed = 4.0f;

    private float xRotate = 0.0f;
	//private float yRotate = 0.0f;

	private void Awake()
	{
		playerBody = transform.root; // 플레이어 본체(부모 오브젝트) 가져오기

		if (!photonView.IsMine)
		{
			// 상대 플레이어 오브젝트 카메라 컴포넌트 비활성화
			OtherCamSetting();
		}
	}

	/// <summary>
	/// 마우스 입력을 이용한 1인칭 카메라 회전 처리
	/// </summary>
	/// <param name="mouseX">마우스 좌/우 값</param>
	/// <param name="mouseY">마우스 상/하 값</param>
	public void CamRotate(float mouseX, float mouseY)
	{
		if (!photonView.IsMine) return;

		float sensitivity = SensitivitySettings.Sensitivity; // 마우스 감도 값 가져오기

		// ★ Mathf.Approximately(a, b)는 a와 b가 거의 같은지 비교하는 함수
		// 마우스가 거의 움직이지 않았으면 회전 생략 (불필요한 연산 방지 = 성능 최적화)
		if (Mathf.Approximately(mouseX, 0f) && Mathf.Approximately(mouseY, 0f)) return;


		// 본체의 Y축 회전: 좌우 회전 처리 (마우스 좌우 이동)
		playerBody.Rotate(Vector3.up * mouseX * sensitivity);


		// 카메라의 X축 회전: 상하 회전 처리 (마우스 위/아래 이동)
		// Unity 기준으로 아래를 볼수록 X축 각도는 증가하므로, mouseY를 반대로 적용
		xRotate -= mouseY * sensitivity;


		// ★ Mathf.Clamp(value, min, max)는 값(value)을 지정된 범위(min ~ max)로 제한하는 함수
		// 플레이어가 고개를 너무 위/아래로 돌리지 못하게 제한
		xRotate = Mathf.Clamp(xRotate, -80, 80);


		// 카메라의 로컬 X축 회전 적용 (좌우는 본체가 처리하므로 0으로 고정)
		transform.localRotation = Quaternion.Euler(xRotate, 0f, 0f);

		////----- ★ FPS에서는 상하 회전은 카메라가, 좌우 회전은 플레이어 본체가 처리하는 구조가 일반적. ★ ------//
	}

	/// <summary>
	/// 다른 플레이어 카메라 및 오디오 리스너 세팅
	/// </summary>
	public void OtherCamSetting()
	{
		if (TryGetComponent<Camera>(out var camera))
		{
			camera.enabled = false; // 상대 플레이어 카메라 비활성화
		}

		if (TryGetComponent<AudioListener>(out var audioListener))
		{
			audioListener.enabled = false;
		}
	}
}
