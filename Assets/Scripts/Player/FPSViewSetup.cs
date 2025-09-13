using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/* �� ȭ�鿡 �� ��üx �� �Ѹ� ���̰� �ϴ� ���� ��ũ��Ʈ */

public class FPSViewSetup : MonoBehaviourPun
{
    /// <summary>
    ///  1��Ī ȭ�鿡 ���� �� ������Ʈ
    /// </summary>
    [SerializeField] private GameObject fpsGun;

    /// <summary>
    /// �÷��̾� �ٵ� ������Ʈ
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
    /// �� ī�޶󿡼��� ��üx 1��Ī ���⸸ ���̰� ����
    /// </summary>
	private void SetupLocalView()
	{
        // FPS ���� �ѱ�
		if (fpsGun != null)
		{
            fpsGun.SetActive(true);
		}

        // �ڽ� ���� ���̾� ������ ���� ī�޶󿡼� �� ���̰�
		if (playerBody != null)
		{
            SetLayerRecursively(playerBody, LayerMask.NameToLayer("PlayerModel"));

            Camera cam = GetComponent<Camera>();
			if (cam != null)
			{
                // �� ���� ī�޶� CullingMask�� PlayerModel ���̾� ����
                cam.cullingMask &= ~(1 << LayerMask.NameToLayer("PlayerModel"));

                /* �� ���� ���� ��
                 * 1 << n : ��Ʈ ����Ʈ ������. �ش� ���̾ ON. // ex) 1 << 8 : 000100000000
                 * ~(...) : ��Ʈ NOT ������. ��Ʈ ����, �� ������ ���̾ ������ ��� ���̰�. // ex) ~(1 << 8) : 111011111111
                 * &= : ��Ʈ AND ������. �� �� 1�̾���� 1, �� ������ ���̾ ������ �������� ����.
                 * 
                 * = �� &= �� : =�� �����. &= ���� ������ �����ϰ� ����� ������ ����.
                 * �� =�� ���� ������ ���õ�. &=�� ������ ������ ������.
                 */
			}
		}
	}

    /// <summary>
    /// �ٸ� �÷��̾�� 1��Ī ���Ⱑ �Ⱥ��̰�
    /// </summary>
    private void SetupRemoteView()
	{
		if (fpsGun != null)
		{
            fpsGun.SetActive(false);
		}
	}

    /// <summary>
    /// ������Ʈ ���̾� �������� ��� ���� �޼ҵ� - ��� Ȯ�� ���ɼ� //
    /// ����Լ��� ����� ���� ������Ʈ���� ��� ���̾� ����
    /// </summary>
    /// <param name="obj">���̾ ������ ������Ʈ</param>
    /// <param name="layer">������ ���̾� ��ȣ</param>
    private void SetLayerRecursively(GameObject obj, int layer)
	{
        obj.layer = layer;

        // �ݺ����� ���� �ڽĵ��� ���̾ ��� ����
        foreach(Transform child in obj.transform)
		{
            SetLayerRecursively(child.gameObject, layer);
		}
	}
}
