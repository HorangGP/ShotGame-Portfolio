using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/*룸 UI 매니저*/

/*
 * 방 정보 표시
 * 게임 시작
 */

public class RoomManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject playerCountTxt; // 방인원 수 텍스트
	[SerializeField] private GameObject gameStartBtn; // 게임 시작 버튼
	[SerializeField] private Text roomTxt; // 방 이름 텍스트
	[SerializeField] private GameObject roomPnl; // 방 UI 패널

	// 표시될 방 인원 수
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
	/// 룸 UI 초기 설정
	/// </summary>
	private void RoomUIStartSetting()
	{
		roomPnl.SetActive(false);
		playerCountTxt.SetActive(false);
		gameStartBtn.GetComponent<Button>().interactable = false;
	}

    /// <summary>
    /// 룸 상태에 따른 UI 업데이트
    /// </summary>
    private void RoomUpdateUI()
	{
        if (PhotonManager.instance.OnJoined)
        {
            // 방 이름 설정
            roomTxt.text = PhotonNetwork.CurrentRoom.Name;

            // 플레이어 수 업데이트
            playerCount = PhotonManager.instance.PlayerCount;

            // 플레이어 수 텍스트 활성화
            playerCountTxt.SetActive(true);

            // Text 컴포넌트가 존재하는 경우 텍스트 업데이트
            if (playerCountTxt.TryGetComponent<Text>(out var textComponent))
            {
                textComponent.text = playerCount + " / 2";
            }
            else
            {
                Debug.LogWarning("Text 컴포넌트를 찾을 수 없습니다.");
            }
        }

        // 플레이어가 꽉차고 && 방 주인이면 게임시작 버튼이 활성화 됨.
        if (gameStartBtn.TryGetComponent<Button>(out var buttonComponent))
        {
            // 조건문을 buttonComponent.interactable을 바로 할당하여 if-else 구조를 간소화 / 안 그러면 if-else 이중으로 들어가야 함.
            buttonComponent.interactable = (playerCount == 2 && PhotonNetwork.IsMasterClient);
        }
        else
        {
            Debug.LogWarning("Button 컴포넌트를 찾을 수 없습니다.");
        }
    }

	// [버튼] - 게임시작 -> GameScene으로 이동
	public void OnGamePlay()
	{
		if (PhotonManager.instance == null)
		{
			Debug.LogError("포톤 매니저 인스턴스가 존재하지 않음.");
			return;
		}

		// 방장만 씬 조작
		if (PhotonNetwork.IsMasterClient == true)
		{
			// RpcTarget.All은 같은 방 사람들 대상으로만 작동됨.
			roomPnl.SetActive(false);
			PhotonManager.instance.photonView.RPC("LoadGame", RpcTarget.All);
		}
		else
		{
			Debug.LogWarning("방장이 아닙니다. 게임을 시작할 수 없습니다.");
		}
	}

	// [버튼] - 방에서 나가기
	public void ExitRoom()
	{
		PhotonNetwork.LeaveRoom(); // 포톤에게 방을 떠난다 알리기
	}

	// 방을 떠날 때 호출
	public override void OnLeftRoom()
	{
		Debug.Log("[OnLeftRoom] 방을 떠납니다.");

		PhotonManager.instance.OnJoined = false; // 방 참가 여부 해제

		// 다음에 연결되면 로비 재입장하도록 설정
		PhotonManager.instance.SetReconnectFlag();

		// 방 화면 비활성화
		roomPnl.SetActive(false);

		// 로비 화면 활성화
		LobbyManager.instance.lobbyPnl.SetActive(true);
		LobbyManager.instance.roomListPnl.SetActive(true);

		LobbyManager.instance.RefreshRoomList(); // 로비로 복귀 시 방 리스트 새로고침
	}
}
