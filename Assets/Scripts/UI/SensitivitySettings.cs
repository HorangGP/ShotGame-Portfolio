using UnityEngine;

/// <summary>
/// 마우스 감도 값을 PlayerPrefs에 저장/불러오는 클래스
/// </summary>
public static class SensitivitySettings
{
	private const string Key = "MouseSensitivity";
	private const float Default = 4.0f;

	/// <summary>
	/// 마우스 감도 값
	/// </summary>
	public static float Sensitivity
	{
		get => PlayerPrefs.GetFloat(Key, Default);
		set => PlayerPrefs.SetFloat(Key, value);
	}

	/// <summary>
	/// PlayerPrefs 저장 강제 실행
	/// </summary>
	public static void Save() => PlayerPrefs.Save();
}
