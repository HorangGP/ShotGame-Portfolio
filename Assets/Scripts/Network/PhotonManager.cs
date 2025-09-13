using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
	// 포톤 매니저 싱글톤
    public static PhotonManager instance;

	// 마스터 서버 연결 여부 -- PhotonNetwork.IsConnected 와 동일
	private bool onConnectedMaster;
	/// <summary>
	/// 마스터 서버 연결 여부
	/// </summary>
	public bool OnConnetedMaster
	{
		set { onConnectedMaster = value; }
		get { return onConnectedMaster; }
	}

	// 방 참가 여부 -- PhotonNetwork.InRoom 와 동일
	private bool onJoined;
	/// <summary>
	/// 방 참가 여부
	/// </summary>
	public bool OnJoined
	{
		set { onJoined = value; }
		get { return onJoined; }
	}

	// 방 인원 수
	private int playerCount;
	/// <summary>
	/// 방 인원 수
	/// </summary>
	public int PlayerCount
	{
		set { playerCount = value; }
		get { return playerCount; }
	}

	/// <summary>
	/// 방 나간 후 재접속 여부
	/// </summary>
	private bool reconnectAfterLeaveRoom = false;

	private void Awake()
	{
		if (instance == null)
		{
            instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		// 시작과 동시에 포톤 연결
		ConnectToPhoton();
	}

	private void Update()
	{
		PlayerCountCheck(); // 방에 있을 때만 인원 수 체크
    }

	/// <summary>
	/// 포톤 서버 연결
	/// </summary>
	public void ConnectToPhoton()
	{
		// 포톤에 연결
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	/// <summary>
	/// 로비에 입장
	/// </summary>
	public void JoinLobby()
	{
		// 마스터 서버에 연결된 상태에서만 로비로 입장
		if (PhotonNetwork.IsConnectedAndReady)
		{
			PhotonNetwork.JoinLobby();
		}
		else
		{
			Debug.LogWarning("[JoinLobby] 포톤 서버에 연결되지 않았습니다. 로비에 입장할 수 없습니다.");
		}
	}

	/// <summary>
	/// 방을 떠난 후 로비에 재접속 플래그 설정
	/// </summary>
	public void SetReconnectFlag()
	{
		reconnectAfterLeaveRoom = true;
	}

	private void PlayerCountCheck()
	{
        // 방에 있을 때만
        if (onJoined && PhotonNetwork.CurrentRoom != null)
        {
            // 방 인원 수 체크
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }

    //---------------------------------------------------------------------------------------------------------------------

    // 클라이언트가 마스터 서버에 연결되고 매치메이킹 및 기타 작업을 수행할 준비가 되면 호출됩니다.
    public override void OnConnectedToMaster()
	{
		onConnectedMaster = true;


        if (reconnectAfterLeaveRoom) // 로비 재입장
		{
			reconnectAfterLeaveRoom = false;
			JoinLobby(); // 로비에 입장
		}
        /*
        * ★ 문제원인 : 방을 나갈 때 {PhotonNetwork.LeaveRoom()} -> 내부적으로 서버 연결이 잠깐 끊켰다가 복구됨.
        *	     결과 : 해당 시점에서 Photon이 마스터 서버와 재연결 중이므로 PhotonNetwork.JoinLobby()가 제대로 작동하지 않았음!
        *	   해결책 : 방에서 나왔을 때만 자동으로 로비 입장 시도.
        */
    }

    // 포톤 서버와의 연결이 끊어진 후 호출됩니다.
    public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.LogError($"Photon 서버 연결 끊킴: {cause}");
	}



    ////---------------------------------------------------------------------------------------------------------------------
    [PunRPC]
	public void LoadGame()
	{
		PhotonNetwork.LoadLevel("GameScene");
	}

	
	public void ExitGame()
	{
		PhotonNetwork.LeaveRoom();
	}
}
