using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

// �̱۸�� �׽�Ʈ

public class TestManager : MonoBehaviourPunCallbacks
{
    public GameObject popupPnl;

    PlayerController controllerTest;
    FirstPersonShot firstPersonShot;

	private void Awake()
	{
        controllerTest = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        firstPersonShot = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<FirstPersonShot>();
    }

	// Start is called before the first frame update
	void Start()
    {
        Debug.Assert(controllerTest);
        Debug.Assert(firstPersonShot);

        controllerTest.EnableControl(true);
        firstPersonShot.EnableShoot(true);

        popupPnl.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
            popupPnl.SetActive(!popupPnl.activeSelf);
		}
    }

    public void OnResume()
	{
        popupPnl.SetActive(false);
    }

	public void OnExit()
	{
        Debug.Log("�Ʒ����� �����ϴ�.");
        SceneManager.LoadScene("TitleScene");
    }
}
