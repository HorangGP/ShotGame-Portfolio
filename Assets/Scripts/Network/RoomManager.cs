using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/*�� UI �Ŵ���*/

/*
 * �� ���� ǥ��
 * ���� ����
 */

public class RoomManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject playerCountTxt; // ���ο� �� �ؽ�Ʈ
	[SerializeField] private GameObject gameStartBtn; // ���� ���� ��ư
	[SerializeField] private Text roomTxt; // �� �̸� �ؽ�Ʈ
	[SerializeField] private GameObject roomPnl; // �� UI �г�

	// ǥ�õ� �� �ο� ��
	private int playerCount = 0;

	// Start is called before the first frame update
	private void Start()
	{
		RoomUIStartSetting();
	}

	// Update is called once per frame
	private void Update()
	{
		RoomUpdateUI();
    }

	/// <summary>
	/// �� UI �ʱ� ����
	/// </summary>
	private void RoomUIStartSetting()
	{
		roomPnl.SetActive(false);
		playerCountTxt.SetActive(false);
		gameStartBtn.GetComponent<Button>().interactable = false;
	}

    /// <summary>
    /// �� ���¿� ���� UI ������Ʈ
    /// </summary>
    private void RoomUpdateUI()
	{
        if (PhotonManager.instance.OnJoined)
        {
            // �� �̸� ����
            roomTxt.text = PhotonNetwork.CurrentRoom.Name;

            // �÷��̾� �� ������Ʈ
            playerCount = PhotonManager.instance.PlayerCount;

            // �÷��̾� �� �ؽ�Ʈ Ȱ��ȭ
            playerCountTxt.SetActive(true);

            // Text ������Ʈ�� �����ϴ� ��� �ؽ�Ʈ ������Ʈ
            if (playerCountTxt.TryGetComponent<Text>(out var textComponent))
            {
                textComponent.text = playerCount + " / 2";
            }
            else
            {
                Debug.LogWarning("Text ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }

        // �÷��̾ ������ && �� �����̸� ���ӽ��� ��ư�� Ȱ��ȭ ��.
        if (gameStartBtn.TryGetComponent<Button>(out var buttonComponent))
        {
            // ���ǹ��� buttonComponent.interactable�� �ٷ� �Ҵ��Ͽ� if-else ������ ����ȭ / �� �׷��� if-else �������� ���� ��.
            buttonComponent.interactable = (playerCount == 2 && PhotonNetwork.IsMasterClient);
        }
        else
        {
            Debug.LogWarning("Button ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

	// [��ư] - ���ӽ��� -> GameScene���� �̵�
	public void OnGamePlay()
	{
		if (PhotonManager.instance == null)
		{
			Debug.LogError("���� �Ŵ��� �ν��Ͻ��� �������� ����.");
			return;
		}

		// ���常 �� ����
		if (PhotonNetwork.IsMasterClient == true)
		{
			// RpcTarget.All�� ���� �� ����� ������θ� �۵���.
			roomPnl.SetActive(false);
			PhotonManager.instance.photonView.RPC("LoadGame", RpcTarget.All);
		}
		else
		{
			Debug.LogWarning("������ �ƴմϴ�. ������ ������ �� �����ϴ�.");
		}
	}

	// [��ư] - �濡�� ������
	public void ExitRoom()
	{
		PhotonNetwork.LeaveRoom(); // ���濡�� ���� ������ �˸���
	}

	// ���� ���� �� ȣ��
	public override void OnLeftRoom()
	{
		Debug.Log("[OnLeftRoom] ���� �����ϴ�.");

		PhotonManager.instance.OnJoined = false; // �� ���� ���� ����

		// ������ ����Ǹ� �κ� �������ϵ��� ����
		PhotonManager.instance.SetReconnectFlag();

		// �� ȭ�� ��Ȱ��ȭ
		roomPnl.SetActive(false);

		// �κ� ȭ�� Ȱ��ȭ
		LobbyManager.instance.lobbyPnl.SetActive(true);
		LobbyManager.instance.roomListPnl.SetActive(true);

		LobbyManager.instance.RefreshRoomList(); // �κ�� ���� �� �� ����Ʈ ���ΰ�ħ
	}
}
