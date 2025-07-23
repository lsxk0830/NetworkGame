using System;
using System.IO;
using Cysharp.Threading.Tasks;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class FacePanelController
{
    private const int targetSize = 256; // 像素大小256*256

    /// <summary>
    /// 跟换头像
    /// </summary>
    public async UniTaskVoid ChangeImage(Image targetImage)
    {
        var extensions = new[] { new ExtensionFilter("Image Files", "png", "jpg", "jpeg") };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);
        if (paths.Length > 0)
        {
            Texture2D texture = await HTTPManager.Instance.GetImage(new Uri(paths[0]).AbsoluteUri);
            if (texture == null)
            {
                PanelManager.Instance.Open<TipPanel>("加载本地图片错误,建议使用.png文件");
                return;
            }

            // 创建临时RenderTexture
            RenderTexture rt = RenderTexture.GetTemporary(targetSize, targetSize);
            Graphics.Blit(texture, rt);
            Texture2D result = new Texture2D(targetSize, targetSize); // 创建目标纹理
            RenderTexture.active = rt; // 从RenderTexture读取
            result.ReadPixels(new Rect(0, 0, targetSize, targetSize), 0, 0);
            result.Apply();
            // 清理
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            targetImage.sprite = Sprite.Create
            (
                result,
                new Rect(0, 0, result.width, result.height),
                new Vector2(0.5f, 0.5f)
            );
            Debug.Log($"本地大小:{result.width},{result.height}");
        }
    }


    /// <summary>
    /// 下载图片
    /// </summary>
    public void DownloadImage(Sprite sprite)
    {
        // 直接获取Sprite关联的Texture
        Texture2D texture = sprite.texture;

        // 创建新Texture2D并复制像素（避免引用原图）
        Texture2D newTex = new Texture2D
        (
            (int)sprite.rect.width,
            (int)sprite.rect.height,
            texture.format,
            texture.mipmapCount > 1
        );
        // 复制有效区域像素
        Color[] pixels = texture.GetPixels
        (
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height
        );
        Debug.Log($"大小:{sprite.rect.width},{(int)sprite.rect.height}");
        newTex.SetPixels(pixels);
        newTex.Apply();
        byte[] _textureBytes = newTex.EncodeToPNG();
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "sample", "png");
        if (!string.IsNullOrEmpty(path))
            File.WriteAllBytesAsync(path, _textureBytes);
    }

    public void Save(Image avatart, Image targetAvatar)
    {
        UploadAvatarData data = new UploadAvatarData()
        {
            ID = GameMain.ID, // 示例ID
            avatarBytes = avatart.sprite.texture.EncodeToPNG()
        };
        // ToDo 上传
        HTTPManager.Instance.Post(API.UploadAvatar, data, onSuccess =>
        {
            Debug.Log($"头像上传成功: {onSuccess}");
            PanelManager.Instance.Open<TipPanel>("头像上传成功");
        },
        (Onfail, StringSplitOptions) =>
        {
            Debug.Log($"头像上传失败: {Onfail}");
            PanelManager.Instance.Open<TipPanel>("头像上传失败");
        }).Forget();
        targetAvatar.sprite = avatart.sprite;
    }
}