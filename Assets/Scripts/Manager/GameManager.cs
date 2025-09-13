using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/*
#1  입장 후 준비 상태:

    플레이어의 조작 비활성화.
    "Ready" 문구 표시.
    플레이어는 뒤돌아 있는 상태.

#2  숫자 카운트다운:

    3 → 2 → 1 표시.
    각 카운트마다 일정 거리 앞으로 이동.

#3  Fire 상태:

    "Fire" 문구 표시 후 조작 활성화.
    "Fire" 문구는 잠시 후 사라짐.
 */

public class GameManager : MonoBehaviourPunCallbacks
{
	public static GameManager Instance = null;

	// 플레이어 프리팹
	[SerializeField] private GameObject playerPrefab;

	// 플레이어가 생성될 위치 배열
	[SerializeField] private Transform[] spawnPositions;

	// UI 메세지
	[SerializeField] private Text gameMessageTxt;

	// 나가기 버튼
	[SerializeField] private GameObject ExitPnl;

	// 에임 표시기 UI
	[SerializeField] private GameObject aimPnl;

	[SerializeField] private List<AudioClip> StageClip; // 스테이지 배경음악

	private List<GameObject> players = new(); // c# 9.0이상 부터 사용가능한 문법 : List<T> list = new(); new List<T>();와 동일.

	private bool isGamePlaying;
	/// <summary>
	/// 게임 진행 여부
	/// </summary>
	public bool IsGamePlaying
	{
		get { return isGamePlaying; }
		set { isGamePlaying = value; }
	}

	//private bool gameStarted = false;   // 게임 시작여부

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject); // 중복된 인스턴스 제거
		}
	}

	// Start is called before the first frame update
	private void Start()
	{
		if (StageClip.Count != 0)
		{
			SoundManager.instance.PlayBGM(StageClip[0], true); // 스테이지 배경음악 재생
		}

		IsGamePlaying = false;
		PlayerSpawnSetting(); // 플레이어 스폰

		ExitPnl.SetActive(false); // 타이틀 나가는 버튼 비활성화
		aimPnl.SetActive(false); // 에임 표시기 Off

		StartCoroutine(GameStartSequence()); // 게임 시작 시퀀스
	}

	/// <summary>
	/// 각 플레이어 스폰
	/// </summary>
	private void PlayerSpawnSetting()
	{
		// 플레이어 연결 확인 및 플레이어 프리팹이 준비 여부 확인
		if (PhotonNetwork.IsConnected && playerPrefab != null)
		{
			// 플레이어 id를 기반으로 위치 결정
			int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
			if (playerIndex < spawnPositions.Length)
			{
				Vector3 spawnPos = spawnPositions[playerIndex].position;
				Quaternion spawnRot = spawnPositions[playerIndex].rotation;

				// 지정된 위치에 플레이어 생성
				GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, spawnRot);
				players.Add(player);

				if (player.TryGetComponent<PlayerState>(out var playerState))
				{
					// 이벤트 구독
					playerState.OnPlayerDied += HandlePlayerDied;
				}
				else
				{
					Debug.LogError("PlayerState 컴포넌트가 없습니다.");
				}
			}
			else
			{
				Debug.LogError("스폰 위치 부족.");
			}
		}
	}

	/// <summary>
	/// 사망한 플레이어의 정보를 처리
	/// </summary>
	/// <param name="player"></param>
	private void HandlePlayerDied(GameObject player)
	{
		if (player.TryGetComponent<PlayerState>(out var playerState) && playerState.IsDie)
		{
			Debug.Log($"플레이어 {player.name} 사망 확인");

			aimPnl.SetActive(false); // 에임 표시기 Off

			if (playerState.photonView.IsMine)
			{
				Debug.Log("로컬 플레이어가 사망");
				UpdateGameMessage("You Lose!");

				photonView.RPC("OtherPlayerWined", RpcTarget.Others);

				photonView.RPC("GameOver", RpcTarget.All); // 게임 오버 처리
			}
		}
	}

	/// <summary>
	/// 게임 오버 처리 - 타이틀 나가는 버튼을 띄우는 함수
	/// </summary>
	[PunRPC]
	private void GameOver()
	{
		ExitPnl.SetActive(true); // 타이틀 나가는 버튼 활성화
	}

	[PunRPC]
	private void OtherPlayerWined()
	{
		UpdateGameMessage("You Win!");
		aimPnl.SetActive(false); // 에임 표시기 Off
	}

	/// <summary>
	/// 게임 시작 시퀀스
	/// </summary>
	/// <returns></returns>
	private IEnumerator GameStartSequence()
	{
		IsGamePlaying = true;
		Debug.Log("게임 시작 시퀀스 시작");

		UpdateGameMessage("");

		yield return new WaitForSeconds(1.0f);

		PlayerController[] players = FindObjectsOfType<PlayerController>();

		// 게임 준비 -- 캐릭터 회전
		UpdateGameMessage("Ready!");

		foreach (var player in players)
		{
			if (player.photonView.IsMine)
			{
				player.photonView.RPC("AutoRotateBackward", RpcTarget.All);
				Debug.Log("로컬 플레이어 회전");
			}
		}

		yield return new WaitForSeconds(1f);


		// 카운트 다운 -- 캐릭터 전진
		for (int i = 3; i > 0; i--)
		{
			UpdateGameMessage(i.ToString());

			foreach (var player in players)
			{
				if (player.photonView.IsMine)
				{
					player.photonView.RPC("AutoForward", RpcTarget.All);
				}

			}

			yield return new WaitForSeconds(1f);

		}

		// 게임 시작 -- 플레이어 조작 해제
		UpdateGameMessage("Fire!!!");

		photonView.RPC("EnableShootControl", RpcTarget.All);
		photonView.RPC("EnablePlayerControl", RpcTarget.All);

		aimPnl.SetActive(true); // 에임 표시기 On

		yield return new WaitForSeconds(0.3f);

		UpdateGameMessage("");
	}

	//private IEnumerator TestSequence()
	//{
	//	IsGamePlaying = true;
	//	Debug.Log("게임 시작 시퀀스 시작");

	//	UpdateGameMessage("");

	//	yield return new WaitForSeconds(1.0f);

	//	ControllerTest[] players = FindObjectsOfType<ControllerTest>();

	//	// 게임 준비 -- 캐릭터 회전
	//	UpdateGameMessage("Ready!");

	//	// 게임 시작 -- 플레이어 조작 해제
	//	UpdateGameMessage("Fire!!!");

	//	aimPnl.SetActive(true); // 에임 표시기 On

	//	yield return new WaitForSeconds(0.3f);

	//	UpdateGameMessage("");
	//}


	/// <summary>
	/// UI 메세지 표시
	/// </summary>
	/// <param name="message">UI에 표시할 메세지</param>
	private void UpdateGameMessage(string message)
	{
		if (gameMessageTxt != null)
		{
			gameMessageTxt.text = message;
		}
	}

	/// <summary>
	/// 플레이어 조작 잠금 해제
	/// </summary>
	[PunRPC]
	private void EnablePlayerControl()
	{
		foreach (var player in FindObjectsOfType<PlayerController>())
		{
			if (player.photonView.IsMine)
			{
				player.EnableControl(true);
			}
		}
	}

	/// <summary>
	/// 플레이어 사격 잠금 해제
	/// </summary>
	[PunRPC]
	private void EnableShootControl()
	{
		foreach (var playerShot in FindObjectsOfType<FirstPersonShot>())
		{
			if (playerShot.photonView.IsMine)
			{
				playerShot.EnableShoot(true);
			}
		}
	}

	/// <summary>
	/// [버튼] 게임 종료
	/// </summary>
	public void ExitGame()
	{
		PhotonManager.instance.ExitGame(); // 포톤 매니저를 통해 게임 종료
	}

	// -------------------------------------------------------------------------------------

	// 방을 나갈 때 호출되는 콜백 함수
	public override void OnLeftRoom()
	{
		PhotonManager.instance.OnJoined = false; // 방 참가 여부 해제

		PhotonNetwork.LoadLevel("TitleScene");
	}
}
