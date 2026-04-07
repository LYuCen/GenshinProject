using System.Linq;
using IUtility;
using UnityEngine;

namespace mView
{
    public class NetworkDebuggerUI : MonoBehaviour
    {
        private float _windowWidth = 350f;
        private float _windowHeight = 450f;
        private float _margin = 20f; // 距离边缘的间距
        private Rect _windowRect;
        private bool _showDetails = true;
        private bool _isInitialized = false; // 用于标记是否已经初始化过位置

        private void OnGUI()
        {
            // 第一次运行时，或者屏幕分辨率改变时，初始化窗口到右上角
            if (!_isInitialized || Event.current.type == EventType.Layout)
            {
                // 计算右上角的坐标：屏幕宽度 - 窗口宽度 - 间距
                float x = Screen.width - _windowWidth - _margin;
                float y = _margin;

                // 如果你希望窗口可以被拖走，只在第一次时设置位置
                if (!_isInitialized)
                {
                    _windowRect = new Rect(x, y, _windowWidth, _windowHeight);
                    _isInitialized = true;
                }
                else
                {
                    // 如果你希望窗口始终死死锁在右上角（即使调整窗口大小），取消下面注释
                    // _windowRect.x = x; 
                    // _windowRect.y = y;
                }
            }

            _windowRect = GUILayout.Window(99, _windowRect, DrawWindow, "网络协议统计工具 (Network Profiler)");
        }

        void DrawWindow(int windowID)
        {
            var monitor = NetworkMonitor.Instance;
            if (!monitor) { GUILayout.Label("Monitor 实例未找到"); return; }

            // --- 窗口内容保持不变 ---
            GUILayout.BeginVertical("box");
            GUILayout.Label($"<b>实时速率:</b>", GUILayout.Width(300));
            GUILayout.Label($"<color=cyan>发送 (OUT):</color> {FormatBytes(monitor.LastSecondSentBytes)}/s");
            GUILayout.Label($"<color=lime>接收 (IN):</color> {FormatBytes(monitor.LastSecondReceivedBytes)}/s");
            GUILayout.EndVertical();

            GUILayout.Space(10);

            _showDetails = GUILayout.Toggle(_showDetails, "显示消息类型详情");

            if (_showDetails)
            {
                GUILayout.Label("<b>发送细分 (按类型):</b>");
                DrawBreakdown(monitor.Sent);
                
                GUILayout.Space(5);
                
                GUILayout.Label("<b>接收细分 (按类型):</b>");
                DrawBreakdown(monitor.Received);
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("重置统计")) monitor.ResetStats();
            
            // 如果你希望窗口固定不能移动，可以删掉下面这行
            GUI.DragWindow();
        }

        // ... DrawBreakdown 和 FormatBytes 方法保持不变 ...
        void DrawBreakdown(NetworkStats stats)
        {
            var sorted = stats.TypeBreakdown.OrderByDescending(x => x.Value).Take(5);
            foreach (var item in sorted)
            {
                float percent = stats.TotalBytes > 0 ? (float)item.Value / stats.TotalBytes * 100f : 0;
                GUILayout.Label($"- {item.Key}: {FormatBytes(item.Value)} ({percent:F1}%)");
            }
        }

        string FormatBytes(float bytes)
        {
            if (bytes > 1024 * 1024) return (bytes / 1024 / 1024).ToString("F2") + " MB";
            if (bytes > 1024) return (bytes / 1024).ToString("F2") + " KB";
            return bytes.ToString("F0") + " B";
        }
    }
}