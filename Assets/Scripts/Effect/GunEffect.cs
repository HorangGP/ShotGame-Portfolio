using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GunEffect : MonoBehaviourPun
{
    [SerializeField] private ParticleSystem muzzleFlash; // ���� �÷��� ����Ʈ
	[SerializeField] private AudioSource gunFireSound; // �� �Ҹ�

    public void PlayMuzzleFlash()
	{
		if (muzzleFlash != null)
		{
            muzzleFlash.Play();
			Debug.Log("���� �÷��� ����Ʈ ���!");
		}
		else
		{
			Debug.LogError("���� �÷��� ����Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");
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
			Debug.Log("����Ʈ �߻�!");
			PlayMuzzleFlash();
			PlayGunFireSound();
		}
	}
}

/*
 * �� ��ġ ��� ȿ���� ���� - <AudioSource>
 *  Spatial Blend�� 3D(1)�� ����
 *  3D Sound Settings�� Min Distance�� Max Distance�� �����Ͽ� �Ÿ� ��� ���� ���� ����
 *  Rolloff Mode ����:
 *  - Logarithmic: �Ҹ��� �־������� �ް��� �پ��(default)
 *  - Linear: �Ÿ� ��ȭ�� ���� ���������� ����
 *  - Custom: Ŀ�� ���� ���� ����
 */