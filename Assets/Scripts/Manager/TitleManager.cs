using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*타이틀 씬*/

/*
 * 게임 시작(로비 연결)
   ㄴ 로비 입장 및 방 생성 기능 및 방 선택 참여 - LobbyManager
        ㄴ 방 입장 및 게임시작 - RoomManager
 * 훈련실(=연습장)
 * 게임 종료
 */

public class TitleManager : MonoBehaviour
{
    public static TitleManager instance;

    public GameObject titlePnl;

    [SerializeField] private GameObject settingPnl; // 설정 패널

	private bool isSettingOpen = false; // 설정 패널 열림 여부

	[SerializeField] private AudioClip titleBgm; // 타이틀 브금
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
        Debug.Log("타이틀 씬 실행");
        InitializeTitle();
    }

    // [버튼] 로비 입장 (타이틀 게임시작 버튼을 누르면 호출)
    public void EnterLobby()
	{
        // 포톤 서버 연결
		if (!PhotonManager.instance.OnConnetedMaster)
		{
            Debug.LogError("아직 서버 연결이 되지 않았습니다. 다시 시도해주세요.");
            return;
		}
        else
        {
			// 로비로 입장
			PhotonManager.instance.JoinLobby();

			// 로비 UI 활성화
			LobbyManager.instance.lobbyPnl.SetActive(true);

			// 타이틀 UI 비활성화
			titlePnl.SetActive(false);
		}
    }

	// [버튼] 설정 패널 열기
	public void OpenSettingPanel()
	{
		isSettingOpen = !isSettingOpen;

		// 설정 패널 활성화
		settingPnl.SetActive(isSettingOpen);
	}

    // [버튼] 게임 종료
    public void QuitGame()
	{
        Application.Quit();
	}

    //------------------------------------------------------------------------
    
    /// <summary>
    /// 초기화 세팅
    /// </summary>
    void InitializeTitle()
    {
        // 로비 UI 비활성화
        LobbyManager.instance.lobbyPnl.SetActive(false);

		isSettingOpen = false; // 설정 패널 닫기

		titlePnl.SetActive(true); // 타이틀 UI 활성화
		settingPnl.SetActive(isSettingOpen); // 설정 패널 비활성화

		// 타이틀 BGM 재생
		if (titleBgm != null)
		{
			SoundManager.instance.PlayBGM(titleBgm, true);
		}
		else
		{
			Debug.LogWarning("타이틀 BGM이 설정되지 않았습니다.");
		}
	}


    //// [버튼] 훈련장 입장
    //// 빌드에서 제대로 작동되지 않으므로 수정 필요
    //public void EnterTrainingRoom()
    //{
    //    Debug.Log("테스트 씬 실행");
    //    SceneManager.LoadScene("TestScene");
    //}
}
