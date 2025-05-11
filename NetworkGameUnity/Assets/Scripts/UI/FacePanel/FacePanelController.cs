using System;
using System.IO;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FacePanelController
{
    private FacePanelModel model;
    private const int targetSize = 256; // 像素大小256*256

    public FacePanelController()
    {
        model = new FacePanelModel();
    }
    public void Show()
    {

    }

    public async UniTaskVoid ChangeImage(Image targetImage)
    {
        // 保存原始工作目录
        string originalDirectory = Directory.GetCurrentDirectory();

        string filePath = await UniTask.RunOnThreadPool(() =>
        {
            try
            {
                OpenFileName ofn = new OpenFileName
                {
                    lStructSize = Marshal.SizeOf<OpenFileName>(),
                    hwndOwner = GetActiveWindow(),
                    lpstrFilter = "Image Files\0*.jpg;*.jpeg;*.png\0All Files\0*.*\0",
                    lpstrFile = new string(new char[256]),
                    nMaxFile = 256,
                    lpstrTitle = "选择图片文件",
                    Flags = 0x00080000 | // OFN_EXPLORER
                            0x00001000 | // OFN_FILEMUSTEXIST
                            0x00000800 | // OFN_NOCHANGEDIR
                            0x00000008
                };

                if (GetOpenFileName(ref ofn))
                {
                    // 提取有效路径（处理多重空终止符）
                    int nullPos = ofn.lpstrFile.IndexOf('\0');
                    string selectedPath = nullPos >= 0 ? ofn.lpstrFile.Substring(0, nullPos) : ofn.lpstrFile;
                    // 恢复工作目录
                    Directory.SetCurrentDirectory(originalDirectory);
                    return selectedPath;
                }
                // 恢复工作目录
                Directory.SetCurrentDirectory(originalDirectory);
                return null;
            }
            catch (Exception ex)
            {
                // 确保在异常情况下也恢复工作目录
                Directory.SetCurrentDirectory(originalDirectory);
                Debug.LogError($"文件对话框错误: {ex.Message}");
                return null;
            }
        });

        if (string.IsNullOrEmpty(filePath)) return;

        try
        {
            // 1. 异步读取文件
            byte[] fileData = await UniTask.RunOnThreadPool(() => File.ReadAllBytes(filePath));
            await UniTask.SwitchToMainThread();
            // 2. 创建原始Texture
            Texture2D sourceTex = new Texture2D(2, 2);
            try
            {
                if (!sourceTex.LoadImage(fileData))
                {
                    Debug.LogError("图片解码失败");
                    UnityEngine.Object.Destroy(sourceTex);
                    return;
                }
                // 3. 计算缩放后尺寸（保持比例）
                float aspect = (float)sourceTex.width / sourceTex.height;
                int scaledWidth, scaledHeight;
                if (aspect > 1) // 宽图
                {
                    scaledWidth = targetSize;
                    scaledHeight = Mathf.RoundToInt(targetSize / aspect);
                }
                else // 高图或方形
                {
                    scaledHeight = targetSize;
                    scaledWidth = Mathf.RoundToInt(targetSize * aspect);
                }
                // 4. 创建目标RenderTexture
                RenderTexture rt = null;
                Texture2D scaledTex = null;
                try
                {
                    rt = RenderTexture.GetTemporary(targetSize, targetSize, 0, RenderTextureFormat.ARGB32);
                    // 5. 缩放绘制到RenderTexture
                    Graphics.Blit(sourceTex, rt);
                    // 6. 创建最终Texture2D
                    scaledTex = new Texture2D(targetSize, targetSize, TextureFormat.RGBA32, false);
                    // 7. 从RenderTexture读取像素
                    RenderTexture.active = rt;
                    scaledTex.ReadPixels(new Rect(0, 0, targetSize, targetSize), 0, 0);
                    scaledTex.Apply();
                    // 8. 创建Sprite
                    Sprite sprite = Sprite.Create(scaledTex, new Rect(0, 0, targetSize, targetSize), new Vector2(0.5f, 0.5f));
                    // 9. 更新Image组件
                    if (targetImage.sprite != null)
                    {
                        UnityEngine.Object.Destroy(targetImage.sprite.texture);
                        UnityEngine.Object.Destroy(targetImage.sprite);
                    }
                    targetImage.sprite = sprite;
                    targetImage.preserveAspect = false;

                    Debug.Log($"已缩放至{targetSize}x{targetSize} (原始: {sourceTex.width}x{sourceTex.height})");
                }
                finally
                {
                    if (rt != null)
                    {
                        RenderTexture.ReleaseTemporary(rt);
                        RenderTexture.active = null;
                    }
                }
            }
            finally
            {
                if (sourceTex != null)
                {
                    UnityEngine.Object.Destroy(sourceTex);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
        }
    }

    public void DownloadImage()
    {

    }
    public void Close()
    {

    }

    #region Windows API声明

    // Windows API声明
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr GetActiveWindow();

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref OpenFileName ofn);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int flagsEx;
    }
    #endregion

}