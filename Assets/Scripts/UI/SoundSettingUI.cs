using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SoundSettingUI : MonoBehaviour
{
	[SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

	// Start is called before the first frame update
	private void OnEnable()
    {
		InitializeSoundUISetting();
	}


	/// <summary>
	/// 사운드 UI 설정 초기화
	/// </summary>
	private void InitializeSoundUISetting()
	{
		masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", SoundManager.instance.defaultMasterVolume);
		bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", SoundManager.instance.defaultBgmVolume);
		sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", SoundManager.instance.defaultSfxVolume);

		masterSlider.onValueChanged.AddListener(SoundManager.instance.SetMasterVolume);
		bgmSlider.onValueChanged.AddListener(SoundManager.instance.SetBgmVolume);
		sfxSlider.onValueChanged.AddListener(SoundManager.instance.SetSfxVolume);
	}
}
