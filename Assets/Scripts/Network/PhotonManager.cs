using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
	// ���� �Ŵ��� �̱���
    public static PhotonManager instance;

	// ������ ���� ���� ���� -- PhotonNetwork.IsConnected �� ����
	private bool onConnectedMaster;
	/// <summary>
	/// ������ ���� ���� ����
	/// </summary>
	public bool OnConnetedMaster
	{
		set { onConnectedMaster = value; }
		get { return onConnectedMaster; }
	}

	// �� ���� ���� -- PhotonNetwork.InRoom �� ����
	private bool onJoined;
	/// <summary>
	/// �� ���� ����
	/// </summary>
	public bool OnJoined
	{
		set { onJoined = value; }
		get { return onJoined; }
	}

	// �� �ο� ��
	private int playerCount;
	/// <summary>
	/// �� �ο� ��
	/// </summary>
	public int PlayerCount
	{
		set { playerCount = value; }
		get { return playerCount; }
	}

	/// <summary>
	/// �� ���� �� ������ ����
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
		// ���۰� ���ÿ� ���� ����
		ConnectToPhoton();
	}

	private void Update()
	{
		PlayerCountCheck(); // �濡 ���� ���� �ο� �� üũ
    }

	/// <summary>
	/// ���� ���� ����
	/// </summary>
	public void ConnectToPhoton()
	{
		// ���濡 ����
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	/// <summary>
	/// �κ� ����
	/// </summary>
	public void JoinLobby()
	{
		// ������ ������ ����� ���¿����� �κ�� ����
		if (PhotonNetwork.IsConnectedAndReady)
		{
			PhotonNetwork.JoinLobby();
		}
		else
		{
			Debug.LogWarning("[JoinLobby] ���� ������ ������� �ʾҽ��ϴ�. �κ� ������ �� �����ϴ�.");
		}
	}

	/// <summary>
	/// ���� ���� �� �κ� ������ �÷��� ����
	/// </summary>
	public void SetReconnectFlag()
	{
		reconnectAfterLeaveRoom = true;
	}

	private void PlayerCountCheck()
	{
        // �濡 ���� ����
        if (onJoined && PhotonNetwork.CurrentRoom != null)
        {
            // �� �ο� �� üũ
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }

    //---------------------------------------------------------------------------------------------------------------------

    // Ŭ���̾�Ʈ�� ������ ������ ����ǰ� ��ġ����ŷ �� ��Ÿ �۾��� ������ �غ� �Ǹ� ȣ��˴ϴ�.
    public override void OnConnectedToMaster()
	{
		onConnectedMaster = true;


        if (reconnectAfterLeaveRoom) // �κ� ������
		{
			reconnectAfterLeaveRoom = false;
			JoinLobby(); // �κ� ����
		}
        /*
        * �� �������� : ���� ���� �� {PhotonNetwork.LeaveRoom()} -> ���������� ���� ������ ��� ���״ٰ� ������.
        *	     ��� : �ش� �������� Photon�� ������ ������ �翬�� ���̹Ƿ� PhotonNetwork.JoinLobby()�� ����� �۵����� �ʾ���!
        *	   �ذ�å : �濡�� ������ ���� �ڵ����� �κ� ���� �õ�.
        */
    }

    // ���� �������� ������ ������ �� ȣ��˴ϴ�.
    public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.LogError($"Photon ���� ���� ��Ŵ: {cause}");
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
