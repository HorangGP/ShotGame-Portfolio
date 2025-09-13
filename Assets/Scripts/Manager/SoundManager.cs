using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // 오디오 믹서 사용을 위한 네임스페이스

/// <summary>
/// 게임 내 사운드 관리 싱글톤 클래스.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

	[Header("오디오 소스")]
	public AudioSource bgmSource; // 배경음악 소스
	public AudioSource sfxSource; // 효과음 소스

	[Header("오디오 믹서")]
	public AudioMixer audioMixer;

	// Mixer에서 Expose(노출)한 파라미터 이름(변경 불가능하게 const로 상수화)
	private const string MASTER_VOLUME_PARAMETER = "MasterVolume";
	private const string BGM_Volume_PARAMETER = "BGMVolume";
	private const string SFX_Volume_PARAMETER = "SFXVolume";


	[Header("기본 볼륨")] // ★ [Range(min, max)] 인스펙터에 슬라이더 UI를 보여짐
	[Range(0f, 1f)] public float defaultMasterVolume = 1f; // 기본 마스터 볼륨
	[Range(0f, 1f)] public float defaultBgmVolume = 1f; // 기본 배경음악 볼륨
	[Range(0f, 1f)] public float defaultSfxVolume = 1f; // 기본 효과음 볼륨


	// SoundManager 싱글톤
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


	// Start is called before the first frame update
	private void Start()
    {
		InitializeAudioSetting(); // 저장된 오디오 설정 불러오기 및 적용
	}

	/// <summary>
	/// BGM 재생
	/// </summary>
	/// <param name="clip">오디오 클립</param>
	/// <param name="loop">루프 여부</param>
	public void PlayBGM(AudioClip clip, bool loop = true)
	{
		if (clip == null) return;

		bgmSource.clip = clip;
		bgmSource.loop = loop;

		bgmSource.Play(); // BGM 재생
	}


	/// <summary>
	/// SFX 재생
	/// </summary>
	/// <param name="clip">오디오 클립</param>
	public void PlaySFX(AudioClip clip)
	{
		if(clip == null) return;

		sfxSource.PlayOneShot(clip); // SFX 재생
	}


	public void SetMasterVolume(float volume)
	{
		float dBVolume = VolumeToDecibel(volume); // 볼륨을 데시벨로 변환
		audioMixer.SetFloat(MASTER_VOLUME_PARAMETER, dBVolume); // 믹서에 마스터 볼륨 설정
		PlayerPrefs.SetFloat("MasterVolume", volume); // 로컬 레지스트리에 마스터 볼륨값 저장
	}


	/// <summary>
	/// BGM 볼륨 설정
	/// </summary>
	/// <param name="volume"></param>
	public void SetBgmVolume(float volume)
	{
		float dBVolume = VolumeToDecibel(volume); // 볼륨을 데시벨로 변환
		audioMixer.SetFloat(BGM_Volume_PARAMETER, dBVolume); // 믹서에 BGM 볼륨 설정
		PlayerPrefs.SetFloat("BGMVolume", volume); // 로컬 레지스트리에 BGM 볼륨값 저장
	}


	/// <summary>
	/// SFX 볼륨 설정
	/// </summary>
	/// <param name="volume"></param>
	public void SetSfxVolume(float volume)
	{
		float dBVolume = VolumeToDecibel(volume); // 볼륨을 데시벨로 변환
		audioMixer.SetFloat(SFX_Volume_PARAMETER, dBVolume); // 믹서에 SFX 볼륨 설정
		PlayerPrefs.SetFloat("SFXVolume", volume); // 로컬 레지스트리에 SFX 볼륨값 저장
	}


	/// <summary>
	/// 로컬 레지스트리에 저장된 사운드 설정 불러오기 및 적용
	/// </summary>
	private void InitializeAudioSetting()
	{
		// 첫 실행 시 로컬 레지스트리에 저장된 볼륨값을 가져옴, 저장된 것이 없으면 설정한 기본값 세팅
		float masterVol = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
		float bgmVol = PlayerPrefs.GetFloat("BGMVolume", defaultBgmVolume);
		float sfxVol = PlayerPrefs.GetFloat("SFXVolume", defaultSfxVolume);

		SetMasterVolume(masterVol);
		SetBgmVolume(bgmVol);
		SetSfxVolume(sfxVol);
	}


	/// <summary>
	/// 볼륨 슬라이더(0 ~ 1) 값을 데시벨로 변환하는 함수 (-80 ~ 0 dB)
	/// </summary>
	/// <param name="volume"></param>
	/// <returns></returns>
	private float VolumeToDecibel(float volume)
	{
		// 0 볼륨은 -80 dB로 간주 (즉 음소거)
		return Mathf.Approximately(volume, 0f) ? -80f : Mathf.Log10(volume) * 20f;

		// ★ Mathf.Log10(volume) * 20f : 사람의 귀는 로그 스케일로 소리를 인식하기 때문에 볼륨을 데시벨로 변환하는 공식.
		// ★ 데시벨(dB) = 20 * log10(volume)
	}
}
