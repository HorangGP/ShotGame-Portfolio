using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 1��Ī ī�޶� ȸ��


// x�� ȸ�� = ���� ȸ��
// y�� ȸ�� = �¿� ȸ��

public class FirstPersonCam : MonoBehaviourPun
{
	/// <summary>
	/// ���� �÷��̾� �ٵ�
	/// </summary>
	private Transform playerBody;

	//[SerializeField] private float turnSpeed = 4.0f;

    private float xRotate = 0.0f;
	//private float yRotate = 0.0f;

	private void Awake()
	{
		playerBody = transform.root; // �÷��̾� ��ü(�θ� ������Ʈ) ��������

		if (!photonView.IsMine)
		{
			// ��� �÷��̾� ������Ʈ ī�޶� ������Ʈ ��Ȱ��ȭ
			OtherCamSetting();
		}
	}

	/// <summary>
	/// ���콺 �Է��� �̿��� 1��Ī ī�޶� ȸ�� ó��
	/// </summary>
	/// <param name="mouseX">���콺 ��/�� ��</param>
	/// <param name="mouseY">���콺 ��/�� ��</param>
	public void CamRotate(float mouseX, float mouseY)
	{
		if (!photonView.IsMine) return;

		float sensitivity = SensitivitySettings.Sensitivity; // ���콺 ���� �� ��������

		// �� Mathf.Approximately(a, b)�� a�� b�� ���� ������ ���ϴ� �Լ�
		// ���콺�� ���� �������� �ʾ����� ȸ�� ���� (���ʿ��� ���� ���� = ���� ����ȭ)
		if (Mathf.Approximately(mouseX, 0f) && Mathf.Approximately(mouseY, 0f)) return;


		// ��ü�� Y�� ȸ��: �¿� ȸ�� ó�� (���콺 �¿� �̵�)
		playerBody.Rotate(Vector3.up * mouseX * sensitivity);


		// ī�޶��� X�� ȸ��: ���� ȸ�� ó�� (���콺 ��/�Ʒ� �̵�)
		// Unity �������� �Ʒ��� ������ X�� ������ �����ϹǷ�, mouseY�� �ݴ�� ����
		xRotate -= mouseY * sensitivity;


		// �� Mathf.Clamp(value, min, max)�� ��(value)�� ������ ����(min ~ max)�� �����ϴ� �Լ�
		// �÷��̾ ���� �ʹ� ��/�Ʒ��� ������ ���ϰ� ����
		xRotate = Mathf.Clamp(xRotate, -80, 80);


		// ī�޶��� ���� X�� ȸ�� ���� (�¿�� ��ü�� ó���ϹǷ� 0���� ����)
		transform.localRotation = Quaternion.Euler(xRotate, 0f, 0f);

		////----- �� FPS������ ���� ȸ���� ī�޶�, �¿� ȸ���� �÷��̾� ��ü�� ó���ϴ� ������ �Ϲ���. �� ------//
	}

	/// <summary>
	/// �ٸ� �÷��̾� ī�޶� �� ����� ������ ����
	/// </summary>
	public void OtherCamSetting()
	{
		if (TryGetComponent<Camera>(out var camera))
		{
			camera.enabled = false; // ��� �÷��̾� ī�޶� ��Ȱ��ȭ
		}

		if (TryGetComponent<AudioListener>(out var audioListener))
		{
			audioListener.enabled = false;
		}
	}
}
