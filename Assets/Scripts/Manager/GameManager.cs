using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/*
#1  ���� �� �غ� ����:

    �÷��̾��� ���� ��Ȱ��ȭ.
    "Ready" ���� ǥ��.
    �÷��̾�� �ڵ��� �ִ� ����.

#2  ���� ī��Ʈ�ٿ�:

    3 �� 2 �� 1 ǥ��.
    �� ī��Ʈ���� ���� �Ÿ� ������ �̵�.

#3  Fire ����:

    "Fire" ���� ǥ�� �� ���� Ȱ��ȭ.
    "Fire" ������ ��� �� �����.
 */

public class GameManager : MonoBehaviourPunCallbacks
{
	public static GameManager Instance = null;

	// �÷��̾� ������
	[SerializeField] private GameObject playerPrefab;

	// �÷��̾ ������ ��ġ �迭
	[SerializeField] private Transform[] spawnPositions;

	// UI �޼���
	[SerializeField] private Text gameMessageTxt;

	// ������ ��ư
	[SerializeField] private GameObject ExitPnl;

	// ���� ǥ�ñ� UI
	[SerializeField] private GameObject aimPnl;

	[SerializeField] private List<AudioClip> StageClip; // �������� �������

	private List<GameObject> players = new(); // c# 9.0�̻� ���� ��밡���� ���� : List<T> list = new(); new List<T>();�� ����.

	private bool isGamePlaying;
	/// <summary>
	/// ���� ���� ����
	/// </summary>
	public bool IsGamePlaying
	{
		get { return isGamePlaying; }
		set { isGamePlaying = value; }
	}

	//private bool gameStarted = false;   // ���� ���ۿ���

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject); // �ߺ��� �ν��Ͻ� ����
		}
	}

	// Start is called before the first frame update
	private void Start()
	{
		if (StageClip.Count != 0)
		{
			SoundManager.instance.PlayBGM(StageClip[0], true); // �������� ������� ���
		}

		IsGamePlaying = false;
		PlayerSpawnSetting(); // �÷��̾� ����

		ExitPnl.SetActive(false); // Ÿ��Ʋ ������ ��ư ��Ȱ��ȭ
		aimPnl.SetActive(false); // ���� ǥ�ñ� Off

		StartCoroutine(GameStartSequence()); // ���� ���� ������
	}

	/// <summary>
	/// �� �÷��̾� ����
	/// </summary>
	private void PlayerSpawnSetting()
	{
		// �÷��̾� ���� Ȯ�� �� �÷��̾� �������� �غ� ���� Ȯ��
		if (PhotonNetwork.IsConnected && playerPrefab != null)
		{
			// �÷��̾� id�� ������� ��ġ ����
			int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
			if (playerIndex < spawnPositions.Length)
			{
				Vector3 spawnPos = spawnPositions[playerIndex].position;
				Quaternion spawnRot = spawnPositions[playerIndex].rotation;

				// ������ ��ġ�� �÷��̾� ����
				GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, spawnRot);
				players.Add(player);

				if (player.TryGetComponent<PlayerState>(out var playerState))
				{
					// �̺�Ʈ ����
					playerState.OnPlayerDied += HandlePlayerDied;
				}
				else
				{
					Debug.LogError("PlayerState ������Ʈ�� �����ϴ�.");
				}
			}
			else
			{
				Debug.LogError("���� ��ġ ����.");
			}
		}
	}

	/// <summary>
	/// ����� �÷��̾��� ������ ó��
	/// </summary>
	/// <param name="player"></param>
	private void HandlePlayerDied(GameObject player)
	{
		if (player.TryGetComponent<PlayerState>(out var playerState) && playerState.IsDie)
		{
			Debug.Log($"�÷��̾� {player.name} ��� Ȯ��");

			aimPnl.SetActive(false); // ���� ǥ�ñ� Off

			if (playerState.photonView.IsMine)
			{
				Debug.Log("���� �÷��̾ ���");
				UpdateGameMessage("You Lose!");

				photonView.RPC("OtherPlayerWined", RpcTarget.Others);

				photonView.RPC("GameOver", RpcTarget.All); // ���� ���� ó��
			}
		}
	}

	/// <summary>
	/// ���� ���� ó�� - Ÿ��Ʋ ������ ��ư�� ���� �Լ�
	/// </summary>
	[PunRPC]
	private void GameOver()
	{
		ExitPnl.SetActive(true); // Ÿ��Ʋ ������ ��ư Ȱ��ȭ
	}

	[PunRPC]
	private void OtherPlayerWined()
	{
		UpdateGameMessage("You Win!");
		aimPnl.SetActive(false); // ���� ǥ�ñ� Off
	}

	/// <summary>
	/// ���� ���� ������
	/// </summary>
	/// <returns></returns>
	private IEnumerator GameStartSequence()
	{
		IsGamePlaying = true;
		Debug.Log("���� ���� ������ ����");

		UpdateGameMessage("");

		yield return new WaitForSeconds(1.0f);

		PlayerController[] players = FindObjectsOfType<PlayerController>();

		// ���� �غ� -- ĳ���� ȸ��
		UpdateGameMessage("Ready!");

		foreach (var player in players)
		{
			if (player.photonView.IsMine)
			{
				player.photonView.RPC("AutoRotateBackward", RpcTarget.All);
				Debug.Log("���� �÷��̾� ȸ��");
			}
		}

		yield return new WaitForSeconds(1f);


		// ī��Ʈ �ٿ� -- ĳ���� ����
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

		// ���� ���� -- �÷��̾� ���� ����
		UpdateGameMessage("Fire!!!");

		photonView.RPC("EnableShootControl", RpcTarget.All);
		photonView.RPC("EnablePlayerControl", RpcTarget.All);

		aimPnl.SetActive(true); // ���� ǥ�ñ� On

		yield return new WaitForSeconds(0.3f);

		UpdateGameMessage("");
	}

	//private IEnumerator TestSequence()
	//{
	//	IsGamePlaying = true;
	//	Debug.Log("���� ���� ������ ����");

	//	UpdateGameMessage("");

	//	yield return new WaitForSeconds(1.0f);

	//	ControllerTest[] players = FindObjectsOfType<ControllerTest>();

	//	// ���� �غ� -- ĳ���� ȸ��
	//	UpdateGameMessage("Ready!");

	//	// ���� ���� -- �÷��̾� ���� ����
	//	UpdateGameMessage("Fire!!!");

	//	aimPnl.SetActive(true); // ���� ǥ�ñ� On

	//	yield return new WaitForSeconds(0.3f);

	//	UpdateGameMessage("");
	//}


	/// <summary>
	/// UI �޼��� ǥ��
	/// </summary>
	/// <param name="message">UI�� ǥ���� �޼���</param>
	private void UpdateGameMessage(string message)
	{
		if (gameMessageTxt != null)
		{
			gameMessageTxt.text = message;
		}
	}

	/// <summary>
	/// �÷��̾� ���� ��� ����
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
	/// �÷��̾� ��� ��� ����
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
	/// [��ư] ���� ����
	/// </summary>
	public void ExitGame()
	{
		PhotonManager.instance.ExitGame(); // ���� �Ŵ����� ���� ���� ����
	}

	// -------------------------------------------------------------------------------------

	// ���� ���� �� ȣ��Ǵ� �ݹ� �Լ�
	public override void OnLeftRoom()
	{
		PhotonManager.instance.OnJoined = false; // �� ���� ���� ����

		PhotonNetwork.LoadLevel("TitleScene");
	}
}
