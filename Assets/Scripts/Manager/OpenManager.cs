using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* ���� �� */

/*
 * ���� ���� �� �ٷ� ����Ǿ�� �� �͵� ����.
 */

public class OpenManager : MonoBehaviour
{
	private void Awake()
	{

	}
	private void Start()
    {
        Debug.Log("���� �� ����");
        SceneManager.LoadScene("TitleScene");
    }
}
