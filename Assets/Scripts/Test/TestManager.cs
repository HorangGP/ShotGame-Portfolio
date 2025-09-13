using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

// ½Ì±Û¸ðµå Å×½ºÆ®

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
        Debug.Log("ÈÆ·ÃÀåÀ» ¶°³³´Ï´Ù.");
        SceneManager.LoadScene("TitleScene");
    }
}
