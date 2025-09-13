using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettingUI : MonoBehaviour
{
	[SerializeField] private Slider sensitivitySlider; // UI에서 연결할 슬라이더
	[SerializeField] private Text sensitivityValueText; // UI에서 연결할 텍스트 (감도 값 표시용)

	private void OnEnable()
	{
		// 시작할 때 슬라이더 값 초기화 (저장된 감도 값을 불러와 적용)
		sensitivitySlider.value = SensitivitySettings.Sensitivity;

		// 슬라이더 값이 변경될 때마다 OnSensitivitySliderChanged 실행
		sensitivitySlider.onValueChanged.AddListener(OnSensitivitySliderChanged);
	}

	/// <summary>
	/// 슬라이더가 변경될 때 실행되는 함수
	/// 현재 감도 값을 저장하고, PlayerPrefs에 저장
	/// </summary>
	/// <param name="value"></param>
	public void OnSensitivitySliderChanged(float value)
	{
		SensitivitySettings.Sensitivity = value;
		SensitivitySettings.Save();
	}
}
