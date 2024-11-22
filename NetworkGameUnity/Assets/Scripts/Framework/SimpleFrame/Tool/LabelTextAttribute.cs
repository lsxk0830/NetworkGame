using UnityEngine;

namespace SimpleFrame
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// 自定义属性，用于指定在Inspector中显示的文本
    /// </summary>
    public class LabelTextAttribute : PropertyAttribute
    {
        public string Text { get; private set; }

        public LabelTextAttribute(string text)
        {
            Text = text;
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// 自定义属性绘制器，用于处理如何在Inspector中显示这个自定义标签
    /// </summary>
    [CustomPropertyDrawer(typeof(LabelTextAttribute))]
    public class LabelTextDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 获取自定义标签的文本
            LabelTextAttribute labelTextAttribute = (LabelTextAttribute)attribute;
            string labelText = labelTextAttribute.Text;
            EditorGUI.PropertyField(position, property, new GUIContent(labelText), true); // 显示字段名称
        }
    }

#endif
}