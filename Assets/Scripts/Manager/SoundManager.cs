using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // ����� �ͼ� ����� ���� ���ӽ����̽�

/// <summary>
/// ���� �� ���� ���� �̱��� Ŭ����.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

	[Header("����� �ҽ�")]
	public AudioSource bgmSource; // ������� �ҽ�
	public AudioSource sfxSource; // ȿ���� �ҽ�

	[Header("����� �ͼ�")]
	public AudioMixer audioMixer;

	// Mixer���� Expose(����)�� �Ķ���� �̸�(���� �Ұ����ϰ� const�� ���ȭ)
	private const string MASTER_VOLUME_PARAMETER = "MasterVolume";
	private const string BGM_Volume_PARAMETER = "BGMVolume";
	private const string SFX_Volume_PARAMETER = "SFXVolume";


	[Header("�⺻ ����")] // �� [Range(min, max)] �ν����Ϳ� �����̴� UI�� ������
	[Range(0f, 1f)] public float defaultMasterVolume = 1f; // �⺻ ������ ����
	[Range(0f, 1f)] public float defaultBgmVolume = 1f; // �⺻ ������� ����
	[Range(0f, 1f)] public float defaultSfxVolume = 1f; // �⺻ ȿ���� ����


	// SoundManager �̱���
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
		InitializeAudioSetting(); // ����� ����� ���� �ҷ����� �� ����
	}

	/// <summary>
	/// BGM ���
	/// </summary>
	/// <param name="clip">����� Ŭ��</param>
	/// <param name="loop">���� ����</param>
	public void PlayBGM(AudioClip clip, bool loop = true)
	{
		if (clip == null) return;

		bgmSource.clip = clip;
		bgmSource.loop = loop;

		bgmSource.Play(); // BGM ���
	}


	/// <summary>
	/// SFX ���
	/// </summary>
	/// <param name="clip">����� Ŭ��</param>
	public void PlaySFX(AudioClip clip)
	{
		if(clip == null) return;

		sfxSource.PlayOneShot(clip); // SFX ���
	}


	public void SetMasterVolume(float volume)
	{
		float dBVolume = VolumeToDecibel(volume); // ������ ���ú��� ��ȯ
		audioMixer.SetFloat(MASTER_VOLUME_PARAMETER, dBVolume); // �ͼ��� ������ ���� ����
		PlayerPrefs.SetFloat("MasterVolume", volume); // ���� ������Ʈ���� ������ ������ ����
	}


	/// <summary>
	/// BGM ���� ����
	/// </summary>
	/// <param name="volume"></param>
	public void SetBgmVolume(float volume)
	{
		float dBVolume = VolumeToDecibel(volume); // ������ ���ú��� ��ȯ
		audioMixer.SetFloat(BGM_Volume_PARAMETER, dBVolume); // �ͼ��� BGM ���� ����
		PlayerPrefs.SetFloat("BGMVolume", volume); // ���� ������Ʈ���� BGM ������ ����
	}


	/// <summary>
	/// SFX ���� ����
	/// </summary>
	/// <param name="volume"></param>
	public void SetSfxVolume(float volume)
	{
		float dBVolume = VolumeToDecibel(volume); // ������ ���ú��� ��ȯ
		audioMixer.SetFloat(SFX_Volume_PARAMETER, dBVolume); // �ͼ��� SFX ���� ����
		PlayerPrefs.SetFloat("SFXVolume", volume); // ���� ������Ʈ���� SFX ������ ����
	}


	/// <summary>
	/// ���� ������Ʈ���� ����� ���� ���� �ҷ����� �� ����
	/// </summary>
	private void InitializeAudioSetting()
	{
		// ù ���� �� ���� ������Ʈ���� ����� �������� ������, ����� ���� ������ ������ �⺻�� ����
		float masterVol = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
		float bgmVol = PlayerPrefs.GetFloat("BGMVolume", defaultBgmVolume);
		float sfxVol = PlayerPrefs.GetFloat("SFXVolume", defaultSfxVolume);

		SetMasterVolume(masterVol);
		SetBgmVolume(bgmVol);
		SetSfxVolume(sfxVol);
	}


	/// <summary>
	/// ���� �����̴�(0 ~ 1) ���� ���ú��� ��ȯ�ϴ� �Լ� (-80 ~ 0 dB)
	/// </summary>
	/// <param name="volume"></param>
	/// <returns></returns>
	private float VolumeToDecibel(float volume)
	{
		// 0 ������ -80 dB�� ���� (�� ���Ұ�)
		return Mathf.Approximately(volume, 0f) ? -80f : Mathf.Log10(volume) * 20f;

		// �� Mathf.Log10(volume) * 20f : ����� �ʹ� �α� �����Ϸ� �Ҹ��� �ν��ϱ� ������ ������ ���ú��� ��ȯ�ϴ� ����.
		// �� ���ú�(dB) = 20 * log10(volume)
	}
}
