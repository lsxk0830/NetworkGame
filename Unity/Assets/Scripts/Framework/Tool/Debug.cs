public static partial class ToolExtension
{
        public static void Log(this object self, string LogStr)
        {
#if UNITY_EDITOR
                UnityEngine.Debug.Log(LogStr);
#endif
        }

        public static void Warning(this object self, string LogStr)
        {
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning(LogStr);
#endif
        }

        public static void Error(this object self, string LogStr)
        {
#if UNITY_EDITOR
                UnityEngine.Debug.LogError(LogStr);
#endif
        }
}