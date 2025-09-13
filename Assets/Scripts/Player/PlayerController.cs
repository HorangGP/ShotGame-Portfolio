using System.Collections;
using System.Collections.Generic;
using Photon.Pun; // ��Ƽ�÷��̸� ���� ���� using
using UnityEngine;

/* �÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ */

public class PlayerController : MonoBehaviourPun
{
	[SerializeField] private FirstPersonCam camController; // 1��Ī ī�޶� ��ũ��Ʈ
	[SerializeField] private FirstPersonShot shotController; // 1��Ī ���� ��ũ��Ʈ

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
		EnableControl(false); // �ʱⰪ�� ���� �Ұ�
	}

	// Update is called once per frame
	private void Update()
    {
		// ���� �÷��̾�(�ڽ�)�� �Է� ó�� + ������ ���۵��� ���� ���¿����� �Է� ó�� ����
		if (!photonView.IsMine || !GameManager.Instance.IsGamePlaying || !canControl) return;

		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		camController?.CamRotate(mouseX, mouseY); // ī�޶� ȸ��

		// ���
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
		{
			shotController?.Shooting();

		}
	}

	//-------------------------------------------------

	/// <summary>
	/// ȣ�� �� �÷��̾� ������ �ӵ��� �ڷ� ����.
	/// </summary>
	[PunRPC]
    public void AutoRotateBackward()
	{
		StopAllCoroutines(); // ��� �ڷ�ƾ ����
		StartCoroutine(RotatePlayer());
		Debug.Log("AutoRotateBackward");
	}

	private IEnumerator RotatePlayer()
	{
        Quaternion startRotation = transform.rotation; // ���۰�
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 180, 0); // ��ǥȸ����

        float duration = 0.5f; // ȸ�� �ð�
        float elapsedTime = 0f; // ��� �ð�

		while (elapsedTime < duration)
		{
			transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
			elapsedTime += Time.deltaTime; // ���������� ������
			yield return null; // �� ������ ��� �� while�� �ٽ� ����
		}

		transform.rotation = targetRotation; // ��Ȯ�� �� ����
	}

    /// <summary>
    /// ȣ�� �� �÷��̾� ������ �ӵ��� ������ ��¦ �̵�.
    /// </summary>
    [PunRPC]
    public void AutoForward()
	{
        StartCoroutine(MoveForwardPlayer());
	}

	private IEnumerator MoveForwardPlayer()
	{
        Vector3 startPos = transform.position; // ���۰�
        Vector3 targetPos = startPos + transform.forward * 0.5f; // ��ǥ �̵���
        float duration = 0.5f; // �̵� �ð�
        float elapsedTime = 0f; // ��� �ð�

        while (elapsedTime < duration)
		{
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
		}

        transform.position = targetPos; // ��Ȯ�� �� ����
    }


	/// <summary>
	/// ���� ���� ����
	/// </summary>
	/// <param name="state"></param>
	public void EnableControl(bool state)
	{
        canControl = state;
	}
}
