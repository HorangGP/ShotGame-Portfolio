using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GunEffect : MonoBehaviourPun
{
    [SerializeField] private ParticleSystem muzzleFlash; // 머즐 플래시 이펙트
	[SerializeField] private AudioSource gunFireSound; // 총 소리

    public void PlayMuzzleFlash()
	{
		if (muzzleFlash != null)
		{
            muzzleFlash.Play();
			Debug.Log("머즐 플래시 이펙트 재생!");
		}
		else
		{
			Debug.LogError("머즐 플래시 이펙트가 할당되지 않았습니다!");
		}
	}

    public void PlayGunFireSound()
	{
		if (gunFireSound != null)
		{
            gunFireSound.Play();
		}
	}

    public void SinglePlayEffect()
	{
        PlayMuzzleFlash();
        PlayGunFireSound();
    }

    [PunRPC]
    public void OtherPlayEffect()
	{
		if (photonView.IsMine)
		{
			Debug.Log("이펙트 발생!");
			PlayMuzzleFlash();
			PlayGunFireSound();
		}
	}
}

/*
 * ★ 위치 기반 효과음 설정 - <AudioSource>
 *  Spatial Blend를 3D(1)로 설정
 *  3D Sound Settings의 Min Distance와 Max Distance를 조정하여 거리 기반 볼륨 감소 설정
 *  Rolloff Mode 설정:
 *  - Logarithmic: 소리가 멀어질수록 급격히 줄어듬(default)
 *  - Linear: 거리 변화에 따라 선형적으로 감소
 *  - Custom: 커브 수동 설정 가능
 */