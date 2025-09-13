using UnityEngine;

/// <summary>
/// ���콺 ���� ���� PlayerPrefs�� ����/�ҷ����� Ŭ����
/// </summary>
public static class SensitivitySettings
{
	private const string Key = "MouseSensitivity";
	private const float Default = 4.0f;

	/// <summary>
	/// ���콺 ���� ��
	/// </summary>
	public static float Sensitivity
	{
		get => PlayerPrefs.GetFloat(Key, Default);
		set => PlayerPrefs.SetFloat(Key, value);
	}

	/// <summary>
	/// PlayerPrefs ���� ���� ����
	/// </summary>
	public static void Save() => PlayerPrefs.Save();
}
