using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 오픈 씬 */

/*
 * 게임 실행 시 바로 진행되어야 할 것들 모음.
 */

public class OpenManager : MonoBehaviour
{
	private void Awake()
	{

	}
	private void Start()
    {
        Debug.Log("오픈 씬 실행");
        SceneManager.LoadScene("TitleScene");
    }
}
