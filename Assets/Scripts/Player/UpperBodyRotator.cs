using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/* ��ü ���� �̵� ��ũ��Ʈ */

public class UpperBodyRotator : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Transform upperBody; // ���� ��ü ��
	[SerializeField] private Camera playerCamera; // ���� �÷��̾� ī�޶�

	private float syncedXRot = 0f; // �ٸ� �÷��̾�κ��� ���� ��ü ȸ�� ��
	private float currentXRot = 0f; // ���� ������ ȸ����



	// ȸ������ �����ϴ� Ÿ�̹��� LateUpdate�� ���� (ī�޶� ȸ�� �� �����ϱ� ����)
	private void LateUpdate()
    {
		if (photonView.IsMine)
		{
			// ���� �÷��̾��� ��ü ȸ������ ������.
			// �� localEulerAngles.x �� ī�޶� ��, �Ʒ��� �󸶳� �������� �˷��ִ� ��.
			float xRot = playerCamera.transform.localEulerAngles.x;

			// ȸ�� ������ 0 ~ 360 ������ ������ ǥ���ǹǷ�, 180���� �Ѿ�� -360���� ���� -180�� ~ 180�� ������ ��ȯ
			if (xRot > 180f)
			{
                xRot -= 360f;
			}

			// ��ü ������Ʈ�� xRot��ŭ ȸ��
            upperBody.localRotation = Quaternion.Euler(xRot, 0f, 0f);
		}
		else
		{
			// �ٸ� �÷��̾�� ���� ȸ�� (Lerp ���� ����)
			currentXRot = Mathf.Lerp(currentXRot, syncedXRot, Time.deltaTime * 10f);

			// ����� ��ü ������Ʈ�� ���� ȸ������ŭ ȸ��
			upperBody.localRotation = Quaternion.Euler(currentXRot, 0f, 0f);
		}
	}



	/// <summary>
	/// PUN�� �ش� �޼��带 ���� �� ������Ʈ�� ���¸� �����ϰų�, �ٸ� ������Ʈ�� ���¸� ����.
	/// </summary>
	/// <param name="stream">���� �Ǵ� ���ſ� ���Ǵ� ��Ʈ��(���)</param>
	/// <param name="info">�޼����� �ΰ�����(���� �ð� ��)</param>
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		// �ش� ������Ʈ�� �ڽ��� ��
		if (stream.IsWriting)
		{
			float xRot = playerCamera.transform.localEulerAngles.x;

			if (xRot > 180f)
			{
				xRot -= 360f;
			}

			// ���� �÷��̾�(��)�� ��ü ȸ������ ���濡�� ����
			stream.SendNext(xRot); 

		}
		else
		{
			// �ٸ� �÷��̾��� ��ü ȸ������ ���Ź޾� ����
			syncedXRot = (float)stream.ReceiveNext(); 
		}
	}
}

/*
 * �� IPunObservable �������̽��� Photon ��Ʈ��ũ���� ��ü�� ���¸� ���������� �ۼ����ϱ� ���� ���.
 * 
 * �� PhotonView �� Observervables�� Synchronization ���� ��
 * 
 * - Unreliable On Change : ���°� ����� ���� ���� (�⺻��), ������ ���� �ٲ�� ���� ������.
 * 
 * - Unreliable : �׻� ���� (�� ������), �������� ���� �սǵ� �� ����.
 * 
 * - Reliable Delta Compressed : ���°� ����� ���� ����� ���� ���� (����), ���� �ٲ�� ���� ����.
 */