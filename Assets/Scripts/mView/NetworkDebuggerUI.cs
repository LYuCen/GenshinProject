using System.Linq;
using Connect;
using IUtility;
using UnityEngine;

namespace mView
{
    public class NetworkDebuggerUI : MonoBehaviour
    {
        private float _windowWidth = 350f;
        private float _windowHeight = 500f; // 稍微调高一点
        private float _margin = 20f;
        private Rect _windowRect;
        private bool _showDetails = true;
        private bool _isInitialized = false;

        private GUIStyle _richTextStyle;
        private GUIStyle _headerStyle;

        private void OnGUI()
        {
            if (!_isInitialized)
            {
                _windowRect = new Rect(Screen.width - _windowWidth - _margin, _margin, _windowWidth, _windowHeight);
                _isInitialized = true;
            }

            // 初始化样式
            if (_richTextStyle == null) {
                _richTextStyle = new GUIStyle(GUI.skin.label) { richText = true, fontSize = 14 };
                _headerStyle = new GUIStyle(GUI.skin.label) { richText = true, fontSize = 16, fontStyle = FontStyle.Bold };
            }

            _windowRect = GUILayout.Window(99, _windowRect, DrawWindow, "网络协议分析器 (Network Profiler)");
        }

        void DrawWindow(int windowID)
        {
            var monitor = NetworkMonitor.Instance;
            if (!monitor) { GUILayout.Label("Monitor 实例未找到"); return; }

            // --- 第一部分：全局运行状态 ---
            GUILayout.BeginVertical("box");
            
            // FPS 统计
            string fpsColor = monitor.CurrentFPS < 30 ? "red" : "yellow";
            GUILayout.Label($"游戏帧率: <color={fpsColor}>{monitor.CurrentFPS:F1} FPS</color>", _richTextStyle);

            // 消息队列统计
            string queueColor = monitor.CurrentQueueCount > 15 ? "red" : "white";
            GUILayout.Label($"消息积压: <color={queueColor}>{monitor.CurrentQueueCount} msgs</color>", _richTextStyle);

            // RTT
            var ping = NetworkService.Instance?.Ping;
            float rtt = ping != null ? ping.RTT * 1000f : 0f;
            string rttColor = rtt > 150f ? "red" : rtt > 80f ? "yellow" : "lime";
            GUILayout.Label($"网络延迟: <color={rttColor}>{rtt:F0} ms</color>", _richTextStyle);

            GUILayout.EndVertical();

            GUILayout.Space(10);

            // --- 第二部分：带宽吞吐 ---
            GUILayout.BeginVertical("box");
            GUILayout.Label("<color=white>实时吞吐量:</color>", _headerStyle);
            // 这里我们手动从 Stats 里拿最近一秒的增量（假设你的 Stats 里有实时速率，没有则直接显示总量）
            GUILayout.Label($"<color=cyan>发送 (UP):</color> {FormatBytes(monitor.Sent.LastSecondBytes)}/s", _richTextStyle);
            GUILayout.Label($"<color=lime>接收 (DOWN):</color> {FormatBytes(monitor.Received.LastSecondBytes)}/s", _richTextStyle);
            GUILayout.EndVertical();

            GUILayout.Space(10);

            // --- 第三部分：协议细分详情 ---
            _showDetails = GUILayout.Toggle(_showDetails, "显示协议详情 (Top 5)");

            if (_showDetails)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label("<b>发送细分:</b>", _richTextStyle);
                DrawBreakdown(monitor.Sent);
                
                GUILayout.Space(5);
                
                GUILayout.Label("<b>接收细分:</b>", _richTextStyle);
                DrawBreakdown(monitor.Received);
                GUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("重置统计数据")) monitor.ResetStats();
            
            GUI.DragWindow();
        }

        void DrawBreakdown(NetworkStats stats)
        {
            // 按流量排序取前5个
            var sorted = stats.TypeBreakdown.OrderByDescending(x => x.Value).Take(5);
            foreach (var item in sorted)
            {
                float percent = stats.TotalBytes > 0 ? (float)item.Value / stats.TotalBytes * 100f : 0;
                GUILayout.Label($"- {item.Key}: {FormatBytes(item.Value)} ({percent:F1}%)", _richTextStyle);
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