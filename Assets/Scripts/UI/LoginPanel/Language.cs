using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;

public class Language : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;

    private void Start()
    {
        languageDropdown = GetComponent<TMP_Dropdown>();
        languageDropdown.onValueChanged.AddListener(selectLanguage);
    }

    public void selectLanguage(int arg0)
    {
        //将下拉框当前选中选项的下标作为参数设置到LocalizationSettings的SelectedLocale达到实现语言切换的效果
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[arg0];
    }
}