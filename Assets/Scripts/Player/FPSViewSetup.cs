using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/* 내 화면에 내 본체x 내 총만 보이게 하는 설정 스크립트 */

public class FPSViewSetup : MonoBehaviourPun
{
    /// <summary>
    ///  1인칭 화면에 보일 총 오브젝트
    /// </summary>
    [SerializeField] private GameObject fpsGun;

    /// <summary>
    /// 플레이어 바디 오브젝트
    /// </summary>
    [SerializeField] private GameObject playerBody;

    // Start is called before the first frame update
    private void Start()
    {
		if (photonView.IsMine)
		{
            SetupLocalView();
		}
		else
		{
            SetupRemoteView();
		}
    }

    /// <summary>
    /// 내 카메라에서는 본체x 1인칭 무기만 보이게 설정
    /// </summary>
	private void SetupLocalView()
	{
        // FPS 무기 켜기
		if (fpsGun != null)
		{
            fpsGun.SetActive(true);
		}

        // 자신 모델을 레이어 변경을 통해 카메라에서 안 보이게
		if (playerBody != null)
		{
            SetLayerRecursively(playerBody, LayerMask.NameToLayer("PlayerModel"));

            Camera cam = GetComponent<Camera>();
			if (cam != null)
			{
                // ★ 현재 카메라 CullingMask에 PlayerModel 레이어 제외
                cam.cullingMask &= ~(1 << LayerMask.NameToLayer("PlayerModel"));

                /* ★ 개념 복습 ★
                 * 1 << n : 비트 쉬프트 연산자. 해당 레이어만 ON. // ex) 1 << 8 : 000100000000
                 * ~(...) : 비트 NOT 연산자. 비트 반전, 즉 선택한 레이어를 제외한 모두 보이게. // ex) ~(1 << 8) : 111011111111
                 * &= : 비트 AND 연산자. 둘 다 1이어야지 1, 즉 선택한 레이어를 제외한 나머지를 유지.
                 * 
                 * = 와 &= 비교 : =는 덮어쓰기. &= 기존 설정을 유지하고 변경된 설정만 변경.
                 * 즉 =는 원래 설정을 무시됨. &=는 나머지 설정을 유지함.
                 */
			}
		}
	}

    /// <summary>
    /// 다른 플레이어는 1인칭 무기가 안보이게
    /// </summary>
    private void SetupRemoteView()
	{
		if (fpsGun != null)
		{
            fpsGun.SetActive(false);
		}
	}

    /// <summary>
    /// 오브젝트 레이어 하위까지 모두 변경 메소드 - 기능 확장 가능성 //
    /// 재귀함수를 사용해 하위 오브젝트까지 모두 레이어 변경
    /// </summary>
    /// <param name="obj">레이어를 변경할 오브젝트</param>
    /// <param name="layer">변경할 레이어 번호</param>
    private void SetLayerRecursively(GameObject obj, int layer)
	{
        obj.layer = layer;

        // 반복문을 통해 자식들의 레이어를 모두 변경
        foreach(Transform child in obj.transform)
		{
            SetLayerRecursively(child.gameObject, layer);
		}
	}
}
