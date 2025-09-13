using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/* 로비 매니저 */

/*
 * 로비 입장 후 방 아이템 생성 및 입장
 * 방 리스트 시각화
 */

public class LobbyManager : MonoBehaviourPunCallbacks
{
	// 로비 매니저 싱글톤
	public static LobbyManager instance;

	// 로비 UI
	public GameObject lobbyPnl;

	// 방 리스트 UI
	public GameObject roomListPnl;

	[SerializeField] private Transform roomListContent;     // 방 리스트 UI의 방 리스트 오브젝트들이 들어갈 공간 (★Transform★)
	//[SerializeField] private GameObject roomListContent;     // 방 리스트 UI의 방 리스트 오브젝트들이 들어갈 공간 (★GameObject★)

	[SerializeField] private GameObject roomListItemPrefab; // 방 리스트 UI의 방 리스트 오브젝트 프리팹
	[SerializeField] private InputField roomNameInput;      // 방 이름 입력 필드
	[SerializeField] private GameObject roomPnl;            // 방 UI

	// 방 리스트를 캐싱하기 위한 딕셔너리
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
		// 이미 타이틀 매니저가 리스트 패널 On/Off 관리중

		// Assert : null 체크 / ★ Debug.Assert()는 False일 때만 실행됨
		// ★ Debug.Assert("")와 Debug.Assert("" != null)은 모두 NULL 일 때 False가 되어 실행되지만 후자가 명확한 코드!
		Debug.Assert(lobbyPnl != null, "로비 UI가 없습니다.");
		Debug.Assert(roomListPnl != null, "방 리스트 UI가 없습니다.");
		Debug.Assert(roomListContent != null, "roomListContent가 없습니다.");
		Debug.Assert(roomListItemPrefab != null, "방 리스트 아이템 프리팹이 없습니다.");
		Debug.Assert(roomNameInput != null, "방 이름 입력 필드가 없습니다.");
		Debug.Assert(roomPnl != null, "방 UI가 없습니다.");
	}

	// [버튼] - 방 만들기
	public void CreateRoom()
	{
		if (PhotonManager.instance.OnConnetedMaster)
		{
			string roomName = roomNameInput.text;
			// 방 이름이 비어있지 않은지 체크
			if (string.IsNullOrEmpty(roomName))
			{
				Debug.LogWarning("방 생성 전에 이름을 입력하세요!");
				return;
			}

			// 방 최대 인원 수 설정 (1 vs 1이기 때문에 2명)
			RoomOptions options = new RoomOptions { MaxPlayers = 2, EmptyRoomTtl = 0 };
			PhotonNetwork.CreateRoom(roomName, options); // 입력한 방 이름대로 방 생성
		}
		else
		{
			Debug.LogError("아직 서버 연결이 되지 않았습니다. 다시 시도해주세요.");
		}
	}

	/// <summary>
	/// 방 입장(방 이름)
	/// </summary>
	/// <param name="roomName"></param>
	public void JoinRoom(string roomName)
	{
		PhotonNetwork.JoinRoom(roomName);
	}

	// 로비로 다시 돌아올 때 강제로 UI 새로고침
	public void RefreshRoomList()
	{
		// 유효하지 않은 방을 캐시에서 제거
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
	/// 방 리스트 UI를 실제로 보여주는 함수
	/// </summary>
	private void UpdateRoomListUI()
	{
		// 기존 방 리스트 초기화
		// 방 리스트의 자식들인 방 오브젝트들을 한번에 찾아 제거해 초기화 하기 위함.
		foreach (Transform content in roomListContent)
		{
			Destroy(content.gameObject);
		}

		// 새로운 방 리스트 표시
		foreach (var room in cacheRoomList.Values)
		{
			// 비어 있는 방을 표시하지 않고 무시하고 지나가게하는 필터링
			if (room.PlayerCount == 0 || room.RemovedFromList) continue;


			// 방 오브젝트를 방 리스트의 자식으로 생성
			GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);


			foreach (Transform child in roomItem.transform)
			{
				// Text 컴포넌트를 자식에서 가져오기 - foreach-TryGetComponent 방식
				if (child.TryGetComponent<Text>(out var roomText))
				{
					roomText.text = $"{room.Name}({room.PlayerCount}/{room.MaxPlayers})";
				}
				else
				{
					Debug.LogWarning($"Text 컴포넌트를 찾을 수 없습니다. Room: {room.Name}");
				}
			}

			// Button 컴포넌트를 가져오기
			if (roomItem.TryGetComponent<Button>(out var roomButton))
			{
				roomButton.onClick.AddListener(() => JoinRoom(room.Name));
			}
			else
			{
				Debug.LogWarning($"Button 컴포넌트를 찾을 수 없습니다. Room: {room.Name}");
			}
		}
	}

	// [버튼] 로비에서 나가기
	public void ExitLobby()
	{
		if (PhotonNetwork.InLobby)
		{
			PhotonNetwork.LeaveLobby(); // 포톤에게 로비를 떠난다 알리기
		}
		else
		{
			Debug.LogWarning("로비에 있지 않아서 LeaveLobby를 호출할 수 없습니다.");
		}
	}

	// -------------------------------------------------------------------------------

	// 서버로부터 방 목록이 갱신될 때 호출
	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		foreach (var room in roomList)
		{
			// 방이 삭제되었거나 플레이어가 없는 경우
			if (room.RemovedFromList || room.PlayerCount == 0)
			{
				// 캐시에서 방 제거
				cacheRoomList.Remove(room.Name);
			}
			else
			{
				// 캐시에 방 정보 업데이트
				cacheRoomList[room.Name] = room;
			}
		}

		// 로비 패널이 활성화되어 있는 경우에만 UI 갱신
		if (lobbyPnl.activeSelf)
		{
			// UI 갱신
			UpdateRoomListUI();
		}
	}

	// 마스터 서버의 로비에 있는 동안 방 리스트의 모든 업데이트를 요청합니다.
	// RoomInfo : 방 리스트로 보여지거나 참여에 필요한 정보만 보유한 간소화 클래스
	//public override void OnRoomListUpdate(List<RoomInfo> roomList)
	//{
	//	Debug.Log($"OnRoomListUpdate 호출됨. 방 개수: {roomList.Count}");

	//	// 기존 방 리스트 초기화
	//	// 방 리스트의 자식들인 방 오브젝트들을 한번에 찾아 제거해 초기화 하기 위함.
	//	//foreach (Transform content in roomListContent.transform)
	//	foreach (Transform content in roomListContent)
	//	{
	//		Destroy(content.gameObject);
	//	}

	//	// 새로운 방 리스트 표시
	//	foreach (var room in roomList)
	//	{
	//		// ★ RoomInfo.RemovedFromList : 방이 삭제된 경우 true
	//		// 삭제된 방이 아닐 때
	//		if (!room.RemovedFromList)
	//		{
	//			// 방 오브젝트를 방 리스트의 자식으로 생성
	//			//GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent.transform);
	//			GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);

	//			//// Text 컴포넌트를 자식에서 가져오기 - 초기 GetComponentInChildren 방식
	//			//roomItem.GetComponentInChildren<Text>().text = $"{room.Name}({room.PlayerCount}/{room.MaxPlayers})";

	//			//// Text 컴포넌트를 자식에서 가져오기 - GetComponentInChildren-null 체크 방식
	//			//Text roomText = roomItem.GetComponentInChildren<Text>();
	//			//if (roomText != null)
	//			//{
	//			//	roomText.text = $"{room.Name}({room.PlayerCount}/{room.MaxPlayers})";
	//			//}
	//			//else
	//			//{
	//			//	Debug.LogWarning($"Text 컴포넌트를 찾을 수 없습니다. Room: {room.Name}");
	//			//}

	//			foreach (Transform child in roomItem.transform)
	//			{
	//				// Text 컴포넌트를 자식에서 가져오기 - foreach-TryGetComponent 방식
	//				if (child.TryGetComponent<Text>(out Text roomText))
	//				{
	//					roomText.text = $"{room.Name}({room.PlayerCount}/{room.MaxPlayers})";
	//				}
	//				else
	//				{
	//					Debug.LogWarning($"Text 컴포넌트를 찾을 수 없습니다. Room: {room.Name}");
	//				}
	//			}

	//			// Button 컴포넌트를 가져오기
	//			if (roomItem.TryGetComponent<Button>(out Button roomButton))
	//			{
	//				roomButton.onClick.AddListener(() => JoinRoom(room.Name));
	//			}
	//			else
	//			{
	//				Debug.LogWarning($"Button 컴포넌트를 찾을 수 없습니다. Room: {room.Name}");
	//			}
	//		}
	//	}
	//}
	/// <summary> 
	/// </summary>

	// 방에 입장 시 호출
	public override void OnJoinedRoom()
	{
		PhotonManager.instance.OnJoined = true;

		lobbyPnl.SetActive(false);
		roomListPnl.SetActive(false);
		roomPnl.SetActive(true);
	}

	// 방 입장 실패 시 호출
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.LogError($"방 입장 실패: {message}");

		PhotonManager.instance.OnJoined = false;
	}

	// 방 생성 실패 시 호출
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.LogError($"방 생성 실패: {message}");
	}

	// 로비를 떠날 때 호출
	public override void OnLeftLobby()
	{

		// 로비 UI 비활성화
		lobbyPnl.SetActive(false);

		// 타이틀 UI 활성화
		TitleManager.instance.titlePnl.SetActive(true);
	}
}
