using System;
using System.Collections.Generic;
using System.IO;
using Connect;
using Connect.Data;
using Connect.Protocol;
using GameFSM.LevelFSM.PlayerFSM;
using mSystem.InputSystem;
using QFramework;
using UnityEngine;

namespace IUtility
{
    public class RunTimeConsoleUtility : MonoBehaviour, IController
    {
        private List<string> _logs = new();
        private List<string> _errorLogs = new();
        [SerializeField] private PlayerManager target;

        private Vector2 _scroll;
        private Rect _windowRect = new Rect(10, 10, 500, 600);
        private bool _needScrollToBottom;

        // --- 新增变量 ---
        private string _inputBuffer = ""; // 输入框缓冲区
        private string _inputFieldControlName = "ConsoleInputField"; // 用于焦点控制

        private IInputSystem _inputSystem;
        private bool _show;
        // 使用属性来包装 _show，确保逻辑同步
        private bool Show
        {
            get => _show;
            set
            {
                if (_show != value)
                {
                    _show = value;
                    // 当控制台显示时，禁用输入系统；隐藏时，启用输入系统
                    if (_inputSystem != null)
                    {
                        _inputSystem.Enable = !_show;
                    }
                    
                    if (_show) 
                    {
                        _inputBuffer = "";
                        // 可选：显示时解锁鼠标，隐藏时锁定鼠标
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }
        }

        // 删除原来的 OnEnable 和 OnDisable 中的 Enable 开关，因为逻辑不对
        void OnEnable() { Application.logMessageReceived += HandleLog; }
        void OnDisable() { Application.logMessageReceived -= HandleLog; }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _logs.Clear();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                // 使用大写的 Show 属性触发逻辑
                Show = !Show;
            }
        }

        void HandleLog(string condition, string stackTrace, LogType type)
        {
            _logs.Add(condition);
            if (_logs.Count > 200) _logs.RemoveAt(0);
            _needScrollToBottom = true;

            if (type == LogType.Error || type == LogType.Exception)
            {
                string errorMsg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {condition}\n{stackTrace}\n";
                _errorLogs.Add(errorMsg);
            }
        }

        // --- 新增方法：解析指令 ---
        void ExecuteCommand(string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd)) return;

            // 记录到日志里，方便看到自己输了什么
            Debug.Log($"> 执行指令: {cmd}");

            
            if (cmd.StartsWith("/connect "))
            {
                // 用法: /connect 26.x.x.x 或 /connect 26.x.x.x:8888
                string addr = cmd.Substring("/connect ".Length).Trim();
                string ip = addr;
                int port = 8888;

                int colonIdx = addr.LastIndexOf(':');
                if (colonIdx > 0)
                {
                    ip = addr.Substring(0, colonIdx);
                    int.TryParse(addr.Substring(colonIdx + 1), out port);
                }

                var ns = NetworkService.Instance;
                if (!ns)
                {
                    Debug.LogError("[指令] NetworkService 未初始化");
                    return;
                }

                ns.serverIp = ip;
                ns.serverPort = port;

                // 如果已连接，先断开
                if (ns.StateMachine.CurrentState == Connect.Core.ConnectionState.Connected)
                {
                    ns.StateMachine.Disconnect();
                }

                ns.ConnectToServer();
                Debug.Log($"<color=cyan>[指令] 正在连接 {ip}:{port} ...</color>");
            }
            else if (cmd.StartsWith("/add bot"))
            {
                // 向模拟器发送一个添加 Bot 的指令
                // 我们不需要传具体信息，让模拟器自己生成坐标和 ID
                NetworkService.Bridge.SendToServer(new Message(MessageType.Local_AddBot, new byte[1024]));
                Debug.Log("<color=yellow>[指令] 请求模拟器添加一个 Bot...</color>");
            }
            else if (cmd.StartsWith("/hit "))
            {
                // 将字符串按空格拆分，并移除多余的空条目
                string[] parts = cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    // parts[0] 是 "/hit"
                    // parts[1] 是 "s" 或 "h"
                    // parts[2] 是 数值
                    if (parts.Length < 3) throw new Exception("参数不足");

                    string type = parts[1].ToLower();
                    float val = float.Parse(parts[2]); // 或者使用你的 .ToFloat()

                    if (!target) 
                    {
                        Debug.LogError("未选中目标！");
                        return;
                    }

                    PlayerStateData data = target.state;

                    if (type == "h")
                    {
                        data.curHp = Mathf.Max(0, data.curHp - val);
                        Debug.Log($"扣除血量: {val}, 剩余: {data.curHp}");
                    }
                    else if (type == "s")
                    {
                        data.curShield = Mathf.Max(0, data.curShield - val);
                        Debug.Log($"扣除护盾: {val}, 剩余: {data.curShield}");
                    }
                    else
                    {
                        throw new Exception("未知类型");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[指令失败] 错误信息: {ex.Message}。正确格式: /hit s 10 或 /hit h 10");
                }
            }
            else if (cmd == "/clear")
            {
                _logs.Clear();
            }
            else
            {
                Debug.LogWarning($"[未知指令] {cmd} 。可用: /connect IP:端口, /add bot, /hit s|h 数值, /clear");
            }
        }

        void OnApplicationQuit()
        {
            ExportErrorLog();
        }

        void ExportErrorLog()
        {
            if (_errorLogs.Count == 0) return;
            try
            {
                string rootPath = Application.persistentDataPath;
                string logFolder = Path.Combine(rootPath, "ErrorLogs");
                if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
                string fileName = $"ErrorLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string fullPath = Path.Combine(logFolder, fileName);
                File.WriteAllLines(fullPath, _errorLogs);
                Debug.Log($"错误日志已导出到: {fullPath}");
            }
            catch (Exception e) { Debug.LogError("导出错误日志失败: " + e.Message); }
        }

        void OnGUI()
        {
            if (!_show) return;
            float width = Screen.width * 0.4f;   // 占屏幕40%
            float height = Screen.height * 0.6f; // 占屏幕60%

            _windowRect.width = width;
            _windowRect.height = height;
            
            _windowRect = GUILayout.Window(0, _windowRect, DrawConsoleWindow, "Runtime Console");
        }

        void DrawConsoleWindow(int windowID)
        {
            // 1. 滚动区域渲染日志
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(_windowRect.height - 100));
            foreach (var log in _logs)
            {
                GUILayout.Label(log);
            }
            GUILayout.EndScrollView();

            // 将滑块滚动到底部
            if (_needScrollToBottom)
            {
                _scroll.y = float.MaxValue;
                _needScrollToBottom = false;
            }

            GUILayout.Space(5);

            // 2. 绘制指令输入区域
            GUILayout.BeginHorizontal();
            
            // 给输入框起个名字，方便监听回车键
            GUI.SetNextControlName(_inputFieldControlName);
            _inputBuffer = GUILayout.TextField(_inputBuffer);

            // 监听键盘回车
            bool hitEnter = Event.current.type == EventType.KeyDown && 
                           (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);

            if (GUILayout.Button("执行", GUILayout.Width(60)) || (hitEnter && GUI.GetNameOfFocusedControl() == _inputFieldControlName))
            {
                ExecuteCommand(_inputBuffer);
                _inputBuffer = ""; // 执行完清空输入框
                GUI.FocusControl(null); // 执行完失去焦点，防止重复触发
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("P 清除日志 | F1 隐藏 | /connect IP:端口 /add bot /hit s|h 数值 /clear");

            // 允许拖动窗口
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}