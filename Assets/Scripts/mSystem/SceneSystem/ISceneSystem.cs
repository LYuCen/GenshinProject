using System;
using System.Threading.Tasks;
using mEvent;
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace mSystem.SceneSystem
{
    public interface ISceneSystem : ISystem
    {
        /// <summary>
        /// 当前加载进度 0~1
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// 加载进度事件
        /// </summary>
        event Action<float> OnProgressChanged;


        /// <summary>
        /// 异步加载场景
        /// </summary>
        Task LoadSceneAsync(string sceneName);
    }

    public class SceneSystem : AbstractSystem, ISceneSystem
    {
        private float _mProgress;
        public float Progress
        {
            get => _mProgress;
            private set
            {
                _mProgress = value;
                OnProgressChanged?.Invoke(value);
            }
        }
        public event Action<float> OnProgressChanged;
        
        protected override void OnInit(){}
        
        public async Task LoadSceneAsync(string sceneName)
        {

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation != null)
            {
                this.SendEvent<LoadSceneStartEvent>();
                // 防止自动进入场景
                operation.allowSceneActivation = false;

                while (operation.progress < 0.9f)
                {
                    float progress = operation.progress * 0.94f / 0.9f;
                    Progress = progress;
                    await Task.Yield();
                }
                // 资源加载完成
                Progress = 1;

                // 等待 300ms 让 UI 显示 100%
                await Task.Delay(300);
                operation.allowSceneActivation = true;
                while (!operation.isDone)
                {
                    await Task.Yield();
                    DynamicGI.UpdateEnvironment();
                    LightProbes.Tetrahedralize();
                }
                this.SendEvent<LoadSceneEndEvent>();
            }
        }
    }
}