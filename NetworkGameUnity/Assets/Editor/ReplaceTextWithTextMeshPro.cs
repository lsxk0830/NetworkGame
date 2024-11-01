using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class ReplaceTextWithTMPText
{
    [MenuItem("Tools/Text 转为 TMP_Text")]
    public static void ReplaceTextComponents()
    {
        string targetFolder = "Assets/Resources"; // 修改为你实际的文件夹路径

        // 获取指定文件夹下的所有Prefab路径
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { targetFolder });

        foreach (string prefabPath in prefabPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabPath);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                if (ReplaceTextInPrefab(prefab))
                    PrefabUtility.SavePrefabAsset(prefab); // 保存Prefab的修改
            }
        }
        AssetDatabase.Refresh();
    }

    private static bool ReplaceTextInPrefab(GameObject prefab)
    {
        bool isSave = false;
        // 获取Prefab中的所有Text组件
        Text[] texts = prefab.GetComponentsInChildren<Text>(true);
        foreach (Text text in texts)
        {
            GameObject go = text.gameObject;

            // 获取原Text组件的属性
            string originalText = text.text;
            float fontSize = text.fontSize;
            Color textColor = text.color;
            TextAnchor anchor = text.alignment; // 对齐方式
            bool bestFit = text.resizeTextForBestFit;
            int minSize = text.resizeTextMinSize;
            int maxSize = text.resizeTextMaxSize;
            float lineSpacing = text.lineSpacing; // 修改为float类型
            bool raycastTarget = text.raycastTarget;

            Object.DestroyImmediate(text, true); // 先移除原有的Text组件

            // 创建TextMeshProUGUI组件
            TextMeshProUGUI tmpText = go.AddComponent<TextMeshProUGUI>();

            // 复制属性到TMP_Text
            tmpText.text = originalText;
            tmpText.fontSize = fontSize;
            tmpText.color = textColor;

            string fontAssetPath = "Assets/TankAsset/Font/华文楷体3500.asset"; // 修改为实际路径
            TMP_FontAsset tmpFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);
            if (tmpFontAsset != null)
            {
                tmpText.font = tmpFontAsset;
            }
            else
            {
                Debug.LogWarning("Failed to load TMP_FontAsset: " + fontAssetPath + ". Please check the path.");
            }
            // 根据原始Text的对齐方式设置TMP_Text的对齐方式
            switch (anchor)
            {
                case TextAnchor.UpperLeft:
                    tmpText.alignment = TextAlignmentOptions.MidlineLeft;
                    break;
                case TextAnchor.UpperCenter:
                    tmpText.alignment = TextAlignmentOptions.Midline;
                    break;
                case TextAnchor.UpperRight:
                    tmpText.alignment = TextAlignmentOptions.MidlineRight;
                    break;
                case TextAnchor.MiddleLeft:
                    tmpText.alignment = TextAlignmentOptions.MidlineLeft;
                    break;
                case TextAnchor.MiddleCenter:
                    tmpText.alignment = TextAlignmentOptions.Midline;
                    break;
                case TextAnchor.MiddleRight:
                    tmpText.alignment = TextAlignmentOptions.MidlineRight;
                    break;
                case TextAnchor.LowerLeft:
                    tmpText.alignment = TextAlignmentOptions.MidlineLeft;
                    break;
                case TextAnchor.LowerCenter:
                    tmpText.alignment = TextAlignmentOptions.Midline;
                    break;
                case TextAnchor.LowerRight:
                    tmpText.alignment = TextAlignmentOptions.MidlineRight;
                    break;
            }

            // 设置最佳适配相关属性
            tmpText.enableAutoSizing = bestFit;
            tmpText.fontSizeMin = minSize;
            tmpText.fontSizeMax = maxSize;
            tmpText.lineSpacing = lineSpacing; // 设置行间距

            // 设置其他属性
            tmpText.raycastTarget = raycastTarget;

            isSave = true;
        }
        return isSave;
    }
}
