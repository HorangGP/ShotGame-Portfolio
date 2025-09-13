using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/* �κ� �Ŵ��� */

/*
 * �κ� ���� �� �� ������ ���� �� ����
 * �� ����Ʈ �ð�ȭ
 */

public class LobbyManager : MonoBehaviourPunCallbacks
{
	// �κ� �Ŵ��� �̱���
	public static LobbyManager instance;

	// �κ� UI
	public GameObject lobbyPnl;

	// �� ����Ʈ UI
	public GameObject roomListPnl;

	[SerializeField] private Transform roomListContent;     // �� ����Ʈ UI�� �� ����Ʈ ������Ʈ���� �� ���� (��Transform��)
	//[SerializeField] private GameObject roomListContent;     // �� ����Ʈ UI�� �� ����Ʈ ������Ʈ���� �� ���� (��GameObject��)

	[SerializeField] private GameObject roomListItemPrefab; // �� ����Ʈ UI�� �� ����Ʈ ������Ʈ ������
	[SerializeField] private InputField roomNameInput;      // �� �̸� �Է� �ʵ�
	[SerializeField] private GameObject roomPnl;            // �� UI

	// �� ����Ʈ�� ĳ���ϱ� ���� ��ųʸ�
	private Dictionary<string, RoomInfo> cacheRoomList = new();

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Start is called before the first frame update
	private void Start()
	{
		// �̹� Ÿ��Ʋ �Ŵ����� ����Ʈ �г� On/Off ������

		// Assert : null üũ / �� Debug.Assert()�� False�� ���� �����
		// �� Debug.Assert("")�� Debug.Assert("" != null)�� ��� NULL �� �� False�� �Ǿ� ��������� ���ڰ� ��Ȯ�� �ڵ�!
		Debug.Assert(lobbyPnl != null, "�κ� UI�� �����ϴ�.");
		Debug.Assert(roomListPnl != null, "�� ����Ʈ UI�� �����ϴ�.");
		Debug.Assert(roomListContent != null, "roomListContent�� �����ϴ�.");
		Debug.Assert(roomListItemPrefab != null, "�� ����Ʈ ������ �������� �����ϴ�.");
		Debug.Assert(roomNameInput != null, "�� �̸� �Է� �ʵ尡 �����ϴ�.");
		Debug.Assert(roomPnl != null, "�� UI�� �����ϴ�.");
	}

	// [��ư] - �� �����
	public void CreateRoom()
	{
		if (PhotonManager.instance.OnConnetedMaster)
		{
			string roomName = roomNameInput.text;
			// �� �̸��� ������� ������ üũ
			if (string.IsNullOrEmpty(roomName))
			{
				Debug.LogWarning("�� ���� ���� �̸��� �Է��ϼ���!");
				return;
			}

			// �� �ִ� �ο� �� ���� (1 vs 1�̱� ������ 2��)
			RoomOptions options = new RoomOptions { MaxPlayers = 2, EmptyRoomTtl = 0 };
			PhotonNetwork.CreateRoom(roomName, options); // �Է��� �� �̸���� �� ����
		}
		else
		{
			Debug.LogError("���� ���� ������ ���� �ʾҽ��ϴ�. �ٽ� �õ����ּ���.");
		}
	}

	/// <summary>
	/// �� ����(�� �̸�)
	/// </summary>
	/// <param name="roomName"></param>
	public void JoinRoom(string roomName)
	{
		PhotonNetwork.JoinRoom(roomName);
	}

	// �κ�� �ٽ� ���ƿ� �� ������ UI ���ΰ�ħ
	public void RefreshRoomList()
	{
		// ��ȿ���� ���� ���� ĳ�ÿ��� ����
		List<string> keysToRemove = new();

		foreach (var pair in cacheRoomList)
		{
			RoomInfo room = pair.Value;

			if (pair.Value.RemovedFromList || pair.Value.PlayerCount == 0)
			{
				keysToRemove.Add(pair.Key);
			}
		}

		foreach (var key in keysToRemove)
		{
			cacheRoomList.Remove(key);
		}

		UpdateRoomListUI();
	}

	/// <summary>
	/// �� ����Ʈ UI�� ������ �����ִ� �Լ�
	/// </summary>
	private void UpdateRoomListUI()
	{
		// ���� �� ����Ʈ �ʱ�ȭ
		// �� ����Ʈ�� �ڽĵ��� �� ������Ʈ���� �ѹ��� ã�� ������ �ʱ�ȭ �ϱ� ����.
		foreach (Transform content in roomListContent)
		{
			Destroy(content.gameObject);
		}

		// ���ο� �� ����Ʈ ǥ��
		foreach (var room in cacheRoomList.Values)
		{
			// ��� �ִ� ���� ǥ������ �ʰ� �����ϰ� ���������ϴ� ���͸�
			if (room.PlayerCount == 0 || room.RemovedFromList) continue;


			// �� ������Ʈ�� �� ����Ʈ�� �ڽ����� ����
			GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);


			foreach (Transform child in roomItem.transform)
			{
				// Text ������Ʈ�� �ڽĿ��� �������� - foreach-TryGetComponent ���
				if (child.TryGetComponent<Text>(out var roomText))
				{
					roomText.text = $"{room.Name}({room.PlayerCount}/{room.MaxPlayers})";
				}
				else
				{
					Debug.LogWarning($"Text ������Ʈ�� ã�� �� �����ϴ�. Room: {room.Name}");
				}
			}

			// Button ������Ʈ�� ��������
			if (roomItem.TryGetComponent<Button>(out var roomButton))
			{
				roomButton.onClick.AddListener(() => JoinRoom(room.Name));
			}
			else
			{
				Debug.LogWarning($"Button ������Ʈ�� ã�� �� �����ϴ�. Room: {room.Name}");
			}
		}
	}

	// [��ư] �κ񿡼� ������
	public void ExitLobby()
	{
		if (PhotonNetwork.InLobby)
		{
			PhotonNetwork.LeaveLobby(); // ���濡�� �κ� ������ �˸���
		}
		else
		{
			Debug.LogWarning("�κ� ���� �ʾƼ� LeaveLobby�� ȣ���� �� �����ϴ�.");
		}
	}

	// -------------------------------------------------------------------------------

	// �����κ��� �� ����� ���ŵ� �� ȣ��
	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		foreach (var room in roomList)
		{
			// ���� �����Ǿ��ų� �÷��̾ ���� ���
			if (room.RemovedFromList || room.PlayerCount == 0)
			{
				// ĳ�ÿ��� �� ����
				cacheRoomList.Remove(room.Name);
			}
			else
			{
				// ĳ�ÿ� �� ���� ������Ʈ
				cacheRoomList[room.Name] = room;
			}
		}

		// �κ� �г��� Ȱ��ȭ�Ǿ� �ִ� ��쿡�� UI ����
		if (lobbyPnl.activeSelf)
		{
			// UI ����
			UpdateRoomListUI();
		}
	}

	// ������ ������ �κ� �ִ� ���� �� ����Ʈ�� ��� ������Ʈ�� ��û�մϴ�.
	// RoomInfo : �� ����Ʈ�� �������ų� ������ �ʿ��� ������ ������ ����ȭ Ŭ����
	//public override void OnRoomListUpdate(List<RoomInfo> roomList)
	//{
	//	Debug.Log($"OnRoomListUpdate ȣ���. �� ����: {roomList.Count}");

	//	// ���� �� ����Ʈ �ʱ�ȭ
	//	// �� ����Ʈ�� �ڽĵ��� �� ������Ʈ���� �ѹ��� ã�� ������ �ʱ�ȭ �ϱ� ����.
	//	//foreach (Transform content in roomListContent.transform)
	//	foreach (Transform content in roomListContent)
	//	{
	//		Destroy(content.gameObject);
	//	}

	//	// ���ο� �� ����Ʈ ǥ��
	//	foreach (var room in roomList)
	//	{
	//		// �� RoomInfo.RemovedFromList : ���� ������ ��� true
	//		// ������ ���� �ƴ� ��
	//		if (!room.RemovedFromList)
	//		{
	//			// �� ������Ʈ�� �� ����Ʈ�� �ڽ����� ����
	//			//GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent.transform);
	//			GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);

	//			//// Text ������Ʈ�� �ڽĿ��� �������� - �ʱ� GetComponentInChildren ���
	//			//roomItem.GetComponentInChildren<Text>().text = $"{room.Name}({room.PlayerCount}/{room.MaxPlayers})";

	//			//// Text ������Ʈ�� �ڽĿ��� �������� - GetComponentInChildren-null üũ ���
	//			//Text roomText = roomItem.GetComponentInChildren<Text>();
	//			//if (roomText != null)
	//			//{
	//			//	roomText.text = $"{room.Name}({room.PlayerCount}/{room.MaxPlayers})";
	//			//}
	//			//else
	//			//{
	//			//	Debug.LogWarning($"Text ������Ʈ�� ã�� �� �����ϴ�. Room: {room.Name}");
	//			//}

	//			foreach (Transform child in roomItem.transform)
	//			{
	//				// Text ������Ʈ�� �ڽĿ��� �������� - foreach-TryGetComponent ���
	//				if (child.TryGetComponent<Text>(out Text roomText))
	//				{
	//					roomText.text = $"{room.Name}({room.PlayerCount}/{room.MaxPlayers})";
	//				}
	//				else
	//				{
	//					Debug.LogWarning($"Text ������Ʈ�� ã�� �� �����ϴ�. Room: {room.Name}");
	//				}
	//			}

	//			// Button ������Ʈ�� ��������
	//			if (roomItem.TryGetComponent<Button>(out Button roomButton))
	//			{
	//				roomButton.onClick.AddListener(() => JoinRoom(room.Name));
	//			}
	//			else
	//			{
	//				Debug.LogWarning($"Button ������Ʈ�� ã�� �� �����ϴ�. Room: {room.Name}");
	//			}
	//		}
	//	}
	//}
	/// <summary> 
	/// </summary>

	// �濡 ���� �� ȣ��
	public override void OnJoinedRoom()
	{
		PhotonManager.instance.OnJoined = true;

		lobbyPnl.SetActive(false);
		roomListPnl.SetActive(false);
		roomPnl.SetActive(true);
	}

	// �� ���� ���� �� ȣ��
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.LogError($"�� ���� ����: {message}");

		PhotonManager.instance.OnJoined = false;
	}

	// �� ���� ���� �� ȣ��
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.LogError($"�� ���� ����: {message}");
	}

	// �κ� ���� �� ȣ��
	public override void OnLeftLobby()
	{

		// �κ� UI ��Ȱ��ȭ
		lobbyPnl.SetActive(false);

		// Ÿ��Ʋ UI Ȱ��ȭ
		TitleManager.instance.titlePnl.SetActive(true);
	}
}
