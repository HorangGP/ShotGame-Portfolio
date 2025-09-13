using System.Collections;
using System.Collections.Generic;
using Photon.Pun; // 멀티플레이를 위한 포톤 using
using UnityEngine;

/* 플레이어 컨트롤러 스크립트 */

public class PlayerController : MonoBehaviourPun
{
	[SerializeField] private FirstPersonCam camController; // 1인칭 카메라 스크립트
	[SerializeField] private FirstPersonShot shotController; // 1인칭 슈팅 스크립트

	private Rigidbody rig;

    private bool canControl = false;


	private void Awake()
	{
		if (TryGetComponent<Rigidbody>(out var rigComponent))
		{
			rig = rigComponent;
		}
	}

	private void Start()
	{
		EnableControl(false); // 초기값은 조작 불가
	}

	// Update is called once per frame
	private void Update()
    {
		// 로컬 플레이어(자신)만 입력 처리 + 게임이 시작되지 않은 상태에서는 입력 처리 안함
		if (!photonView.IsMine || !GameManager.Instance.IsGamePlaying || !canControl) return;

		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		camController?.CamRotate(mouseX, mouseY); // 카메라 회전

		// 사격
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
		{
			shotController?.Shooting();

		}
	}

	//-------------------------------------------------

	/// <summary>
	/// 호출 시 플레이어 적당한 속도로 뒤로 돌기.
	/// </summary>
	[PunRPC]
    public void AutoRotateBackward()
	{
		StopAllCoroutines(); // 모든 코루틴 정지
		StartCoroutine(RotatePlayer());
		Debug.Log("AutoRotateBackward");
	}

	private IEnumerator RotatePlayer()
	{
        Quaternion startRotation = transform.rotation; // 시작값
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 180, 0); // 목표회전값

        float duration = 0.5f; // 회전 시간
        float elapsedTime = 0f; // 경과 시간

		while (elapsedTime < duration)
		{
			transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
			elapsedTime += Time.deltaTime; // 정상적으로 증가함
			yield return null; // 한 프레임 대기 후 while문 다시 실행
		}

		transform.rotation = targetRotation; // 정확한 값 보정
	}

    /// <summary>
    /// 호출 시 플레이어 적당한 속도로 앞으로 살짝 이동.
    /// </summary>
    [PunRPC]
    public void AutoForward()
	{
        StartCoroutine(MoveForwardPlayer());
	}

	private IEnumerator MoveForwardPlayer()
	{
        Vector3 startPos = transform.position; // 시작값
        Vector3 targetPos = startPos + transform.forward * 0.5f; // 목표 이동값
        float duration = 0.5f; // 이동 시간
        float elapsedTime = 0f; // 경과 시간

        while (elapsedTime < duration)
		{
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
		}

        transform.position = targetPos; // 정확한 값 보정
    }


	/// <summary>
	/// 조작 가능 여부
	/// </summary>
	/// <param name="state"></param>
	public void EnableControl(bool state)
	{
        canControl = state;
	}
}
