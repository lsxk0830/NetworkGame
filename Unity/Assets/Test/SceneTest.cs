// using UnityEngine;
// using Cysharp.Threading.Tasks;

// public class SceneTest : MonoBehaviour
// {
//     void Start()
//     {
//         SceneManagerAsync.Instance.SetLoadingProgressCallback(progress =>
//         {
//             Debug.Log($"加载进度: {progress * 100}%");
//             // 更新UI进度条等
//         });
//         SceneManagerAsync.Instance.LoadSceneAsync("Game", true).Forget();
//     }
// }