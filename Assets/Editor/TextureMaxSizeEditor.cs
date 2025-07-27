using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class TextureMaxSizeEditor : EditorWindow
{
    private List<Texture2D> _textures = new List<Texture2D>();
    private int _maxSize = 1024;
    private Vector2 _scrollPos;

    [MenuItem("Tools/批量纹理尺寸调整")]
    public static void ShowWindow() => GetWindow<TextureMaxSizeEditor>("纹理尺寸批量设置");

    private void OnGUI()
    {
        // 1. 操作按钮区域
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("清空列表", GUILayout.Width(100)))
            _textures.Clear();

        if (GUILayout.Button("应用设置", GUILayout.Width(100)) && _textures.Count > 0)
            ApplySettings();
        EditorGUILayout.EndHorizontal();

        // 2. 拖拽区域（支持多次拖入）
        Rect dropArea = GUILayoutUtility.GetRect(0, 150, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "拖拽纹理至此区域（可多次拖入）");
        HandleDragEvents(dropArea);

        // 3. 尺寸设置滑块
        _maxSize = EditorGUILayout.IntSlider("最大尺寸", _maxSize, 32, 8192);

        // 4. 显示已拖入纹理列表（带滚动条）
        GUILayout.Label($"已选纹理: {_textures.Count}");
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, "box");
        foreach (var tex in _textures)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(tex), GUILayout.Width(50), GUILayout.Height(50));
            EditorGUILayout.LabelField(tex.name);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    // 处理拖拽事件（支持多纹理）
    private void HandleDragEvents(Rect dropArea)
    {
        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition)) return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    // 过滤并添加新纹理（避免重复）
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        if (obj is Texture2D tex && !_textures.Contains(tex))
                            _textures.Add(tex);
                    }
                }
                evt.Use();
                break;
        }
    }

    // 批量修改纹理尺寸（优化性能）
    private void ApplySettings()
    {
        AssetDatabase.StartAssetEditing(); // 避免频繁刷新
        try
        {
            for (int i = 0; i < _textures.Count; i++)
            {
                string path = AssetDatabase.GetAssetPath(_textures[i]);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null || importer.maxTextureSize == _maxSize) continue;

                importer.maxTextureSize = _maxSize;
                importer.SaveAndReimport(); // 重新导入生效
                EditorUtility.DisplayProgressBar("处理中", $"修改: {_textures[i].name}", (float)i / _textures.Count);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            Debug.Log($"成功修改 {_textures.Count} 张纹理尺寸！");
        }
    }
}