using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    private Toggle Toggle_BG;
    private Toggle Toggle_Effect;
    private Slider Slider_BG;
    private Slider Slider_Effect;
    public Button BtnClose;

    public override void OnInit()
    {
        layer = PanelManager.Layer.Panel;
        // 寻找组件
        Toggle_BG = transform.Find("Toggle_BG").GetComponent<Toggle>();
        Toggle_Effect = transform.Find("Toggle_Effect").GetComponent<Toggle>();
        Slider_BG = transform.Find("Slider_BG").GetComponent<Slider>();
        Slider_Effect = transform.Find("Slider_Effect").GetComponent<Slider>();
        BtnClose = transform.Find("BtnClose").GetComponent<Button>();
    }

    public override void OnShow(params object[] para)
    {
        gameObject.SetActive(true);

        Toggle_BG.isOn = PlayerPrefs.GetInt("Toggle_BG") == 1 ? true : false;
        Toggle_Effect.isOn = PlayerPrefs.GetInt("Toggle_Effect") == 1 ? true : false;
        Slider_BG.value = PlayerPrefs.GetFloat("Slider_BG");
        Slider_Effect.value = PlayerPrefs.GetFloat("Slider_Effect");

        Toggle_BG.onValueChanged.AddListener(ToggleMusic);
        Toggle_Effect.onValueChanged.AddListener(ToggleSound);
        Slider_BG.onValueChanged.AddListener(SliderMusic);
        Slider_Effect.onValueChanged.AddListener(SliderSound);
        BtnClose.onClick.AddListener(OnClose);
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
        Toggle_BG.onValueChanged.RemoveListener(ToggleMusic);
        Toggle_Effect.onValueChanged.RemoveListener(ToggleSound);
        Slider_BG.onValueChanged.RemoveListener(SliderMusic);
        Slider_Effect.onValueChanged.RemoveListener(SliderSound);
        BtnClose.onClick.RemoveListener(OnClose);
    }

    #region UI事件

    private void ToggleMusic(bool arg)
    {
        BGMusicManager.Instance.ChangeOpen(arg);
        PlayerPrefs.SetInt("Toggle_BG", arg ? 1 : 0);
    }
    private void ToggleSound(bool arg)
    {
        PlayerPrefs.SetInt("Toggle_Effect", arg ? 1 : 0);
    }

    private void SliderMusic(float f)
    {
        BGMusicManager.Instance.ChangeValue(f);
        PlayerPrefs.SetFloat("Slider_BG", f);
    }

    private void SliderSound(float f)
    {
        PlayerPrefs.SetFloat("Slider_Effect", f);
    }

    #endregion
}