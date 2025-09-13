using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*Ÿ��Ʋ ��*/

/*
 * ���� ����(�κ� ����)
   �� �κ� ���� �� �� ���� ��� �� �� ���� ���� - LobbyManager
        �� �� ���� �� ���ӽ��� - RoomManager
 * �Ʒý�(=������)
 * ���� ����
 */

public class TitleManager : MonoBehaviour
{
    public static TitleManager instance;

    public GameObject titlePnl;

    [SerializeField] private GameObject settingPnl; // ���� �г�

	private bool isSettingOpen = false; // ���� �г� ���� ����

	[SerializeField] private AudioClip titleBgm; // Ÿ��Ʋ ���
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
	void Start()
    {
        Debug.Log("Ÿ��Ʋ �� ����");
        InitializeTitle();
    }

    // [��ư] �κ� ���� (Ÿ��Ʋ ���ӽ��� ��ư�� ������ ȣ��)
    public void EnterLobby()
	{
        // ���� ���� ����
		if (!PhotonManager.instance.OnConnetedMaster)
		{
            Debug.LogError("���� ���� ������ ���� �ʾҽ��ϴ�. �ٽ� �õ����ּ���.");
            return;
		}
        else
        {
			// �κ�� ����
			PhotonManager.instance.JoinLobby();

			// �κ� UI Ȱ��ȭ
			LobbyManager.instance.lobbyPnl.SetActive(true);

			// Ÿ��Ʋ UI ��Ȱ��ȭ
			titlePnl.SetActive(false);
		}
    }

	// [��ư] ���� �г� ����
	public void OpenSettingPanel()
	{
		isSettingOpen = !isSettingOpen;

		// ���� �г� Ȱ��ȭ
		settingPnl.SetActive(isSettingOpen);
	}

    // [��ư] ���� ����
    public void QuitGame()
	{
        Application.Quit();
	}

    //------------------------------------------------------------------------
    
    /// <summary>
    /// �ʱ�ȭ ����
    /// </summary>
    void InitializeTitle()
    {
        // �κ� UI ��Ȱ��ȭ
        LobbyManager.instance.lobbyPnl.SetActive(false);

		isSettingOpen = false; // ���� �г� �ݱ�

		titlePnl.SetActive(true); // Ÿ��Ʋ UI Ȱ��ȭ
		settingPnl.SetActive(isSettingOpen); // ���� �г� ��Ȱ��ȭ

		// Ÿ��Ʋ BGM ���
		if (titleBgm != null)
		{
			SoundManager.instance.PlayBGM(titleBgm, true);
		}
		else
		{
			Debug.LogWarning("Ÿ��Ʋ BGM�� �������� �ʾҽ��ϴ�.");
		}
	}


    //// [��ư] �Ʒ��� ����
    //// ���忡�� ����� �۵����� �����Ƿ� ���� �ʿ�
    //public void EnterTrainingRoom()
    //{
    //    Debug.Log("�׽�Ʈ �� ����");
    //    SceneManager.LoadScene("TestScene");
    //}
}
