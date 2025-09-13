using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettingUI : MonoBehaviour
{
	[SerializeField] private Slider sensitivitySlider; // UI���� ������ �����̴�
	[SerializeField] private Text sensitivityValueText; // UI���� ������ �ؽ�Ʈ (���� �� ǥ�ÿ�)

	private void OnEnable()
	{
		// ������ �� �����̴� �� �ʱ�ȭ (����� ���� ���� �ҷ��� ����)
		sensitivitySlider.value = SensitivitySettings.Sensitivity;

		// �����̴� ���� ����� ������ OnSensitivitySliderChanged ����
		sensitivitySlider.onValueChanged.AddListener(OnSensitivitySliderChanged);
	}

	/// <summary>
	/// �����̴��� ����� �� ����Ǵ� �Լ�
	/// ���� ���� ���� �����ϰ�, PlayerPrefs�� ����
	/// </summary>
	/// <param name="value"></param>
	public void OnSensitivitySliderChanged(float value)
	{
		SensitivitySettings.Sensitivity = value;
		SensitivitySettings.Save();
	}
}
