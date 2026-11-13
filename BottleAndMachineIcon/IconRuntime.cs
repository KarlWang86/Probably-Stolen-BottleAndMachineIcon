using System;
using UnityEngine;

namespace BottleAndMachineIcon
{
    internal static partial class IconRuntime
    {
        static ModLog log;
        static GameObject pump;
        static PlayerStore store;
        internal static bool Enabled { get; private set; }

        internal static void Init(ModManifest manifest)
        {
            log = new ModLog(manifest);
            log.Log("容器与机器图标 " + typeof(IconRuntime).Assembly.GetName().Version.ToString(3) + " 初始化。");
        }

        internal static void Enable()
        {
            if (Enabled) return;
            ResetSession();
            Enabled = true;
            ModHook.OnGameLoadedNormal += ResetSession;
            pump = new GameObject("BottleAndMachineIcon.Pump");
            UnityEngine.Object.DontDestroyOnLoad(pump);
            pump.AddComponent<IconPump>();
            log?.Log("已启用只读图标；请先停用其他模组的同类绘制，避免重复。");
        }

        internal static void Disable()
        {
            Enabled = false;
            ModHook.OnGameLoadedNormal -= ResetSession;
            ResetSession();
            if (pump != null) UnityEngine.Object.Destroy(pump);
            pump = null;
        }

        static void ResetSession()
        {
            ClearMaterialIcons();
            store = null;
        }

        internal static void Tick()
        {
            if (!Enabled) return;
            var current = PlayerStore.IsInstanceExist() ? PlayerStore.Instance : null;
            if (!ReferenceEquals(current, store))
            {
                ResetSession();
                store = current;
            }
            TickMaterialIcons();
        }

        internal static void Recover(Exception error)
        {
            ResetSession();
            Debug.LogWarning("[BottleAndMachineIcon] 图标刷新失败，稍后重试：" + error.Message);
        }
    }

    public sealed class IconPump : MonoBehaviour
    {
        float retryAt;
        // LateUpdate reads the native image's final transform/visibility for this frame.
        void LateUpdate()
        {
            if (!IconRuntime.Enabled || Time.unscaledTime < retryAt) return;
            try { IconRuntime.Tick(); }
            catch (Exception error)
            {
                retryAt = Time.unscaledTime + 5f;
                IconRuntime.Recover(error);
            }
        }
    }
}
