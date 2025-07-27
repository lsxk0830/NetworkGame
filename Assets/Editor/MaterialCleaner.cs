using UnityEditor;
using UnityEngine;

public class MaterialCleaner : EditorWindow
{
    [MenuItem("Tools/清除材质残留引用")]
    static void CleanMaterials()
    {
        // 1. 选中需清理的材质（此处以Picture_4.mat为例）
        Material targetMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/TankAsset/EffectTool/Suntail Village/Materials/Props/Picture_4.mat");

        // 2. 创建新材质（仅保留当前Shader声明的属性）
        Material newMat = new Material(targetMat.shader);
        int propCount = ShaderUtil.GetPropertyCount(targetMat.shader);

        // 3. 复制有效属性
        for (int i = 0; i < propCount; i++)
        {
            string propName = ShaderUtil.GetPropertyName(targetMat.shader, i);
            ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(targetMat.shader, i);
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    newMat.SetTexture(propName, targetMat.GetTexture(propName)); // 仅复制当前有效纹理
                    break;
                    // 其他类型（Color/Vector等）同理
            }
        }

        // 4. 替换原材质并保存
        EditorUtility.CopySerialized(newMat, targetMat);
        EditorUtility.SetDirty(targetMat);
        AssetDatabase.SaveAssets();
        Debug.Log($"材质 {targetMat.name} 残留引用已清除");
    }
}