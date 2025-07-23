using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    private Toggle Toggle_Music;
    private Toggle Toggle_Sound;
    private Slider Slider_Music;
    private Slider Slider_Sound;
    public Button BtnClose;

    public override void OnInit()
    {
        layer = PanelManager.Layer.Panel;
        // 寻找组件
        Toggle_Music = transform.Find("Toggle_Music").GetComponent<Toggle>();
        Toggle_Sound = transform.Find("Toggle_Sound").GetComponent<Toggle>();
        Slider_Music = transform.Find("Slider_Music").GetComponent<Slider>();
        Slider_Sound = transform.Find("Slider_Sound").GetComponent<Slider>();
        BtnClose = transform.Find("BtnClose").GetComponent<Button>();
    }

    public override void OnShow(params object[] para)
    {
        gameObject.SetActive(true);

        Toggle_Music.isOn = PlayerPrefs.GetInt("Toggle_Music") == 1 ? true : false;
        Toggle_Sound.isOn = PlayerPrefs.GetInt("Toggle_Sound") == 1 ? true : false;
        Slider_Music.value = PlayerPrefs.GetFloat("Slider_Music");
        Slider_Sound.value = PlayerPrefs.GetFloat("Slider_Sound");

        Toggle_Music.onValueChanged.AddListener(ToggleMusic);
        Toggle_Sound.onValueChanged.AddListener(ToggleSound);
        Slider_Music.onValueChanged.AddListener(SliderMusic);
        Slider_Sound.onValueChanged.AddListener(SliderSound);
        BtnClose.onClick.AddListener(OnClose);
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
        Toggle_Music.onValueChanged.RemoveListener(ToggleMusic);
        Toggle_Sound.onValueChanged.RemoveListener(ToggleSound);
        Slider_Music.onValueChanged.RemoveListener(SliderMusic);
        Slider_Sound.onValueChanged.RemoveListener(SliderSound);
        BtnClose.onClick.RemoveListener(OnClose);
    }

    #region UI事件

    private void ToggleMusic(bool arg)
    {
        BGMusicManager.Instance.ChangeOpen(arg);
        PlayerPrefs.SetInt("Toggle_Music", arg ? 1 : 0);
    }
    private void ToggleSound(bool arg)
    {
        PlayerPrefs.SetInt("Toggle_Sound", arg ? 1 : 0);
    }

    private void SliderMusic(float f)
    {
        BGMusicManager.Instance.ChangeValue(f);
        PlayerPrefs.SetFloat("Slider_Music", f);
    }

    private void SliderSound(float f)
    {
        PlayerPrefs.SetFloat("Slider_Sound", f);
    }

    #endregion
}