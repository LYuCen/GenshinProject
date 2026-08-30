using System.Diagnostics;
using BaseClass;
using UnityEngine;

namespace Connect
{
    public class DedicatedServerLauncher : AbstractSingleton<DedicatedServerLauncher>
    {
        private static string ServerDir =>
            System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "..", "..", "Server"));


        private Process _process;

        protected override void Awake()
        {
            base.Awake();
            Application.quitting += Kill;
        }

        public void Launch()
        {
            if(_process != null && !_process.HasExited) return;

            var batPath = System.IO.Path.Combine(ServerDir, "startServerWait.bat");
            var psi = new ProcessStartInfo
            {
                FileName = batPath,
                WorkingDirectory = ServerDir,
                UseShellExecute = true,
            };
            _process = Process.Start(psi);
            UnityEngine.Debug.Log($"[ServerLauncher] 服务器进程启动PID={_process.Id}");

        }

        public void Kill()
        {
            if (_process == null || _process.HasExited)
            {
                _process = null;
                return;
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = $"/PID {_process.Id} /T /F",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                Process.Start(psi)?.WaitForExit();
            }
            catch
            {
                // ignored
            }

            _process = null;
        }

        protected override void OnDestroy()
        {
            Application.quitting -= Kill;
            base.OnDestroy();
        }

        public bool IsServerRunning(int timeoutMs = 400)
        {
            try
            {
                using var client = new System.Net.Sockets.TcpClient();
                var ar = client.BeginConnect(
                    NetworkService.Instance.serverIp,
                    NetworkService.Instance.serverPort, null, null);

                if (!ar.AsyncWaitHandle.WaitOne(timeoutMs))
                {
                    client.Close();
                    return false;
                }

                client.EndConnect(ar);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}