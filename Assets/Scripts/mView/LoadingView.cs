using BaseClass;
using mEvent;
using mSystem.SceneSystem;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace mView
{
    public class LoadingView : AbstractSingleton<LoadingView>, IController
    {
        [SerializeField] private Image loading;
        [SerializeField] private Image background;

        private ISceneSystem _sceneSystem;

        private void Start()
        {
            this.RegisterEvent<LoadSceneEndEvent>(_ =>
            {
                background.gameObject.SetActive(false);
                loading.fillAmount = 0;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<LoadSceneStartEvent>(_ =>
            {
                if (_sceneSystem == null)
                {
                    _sceneSystem = this.GetSystem<ISceneSystem>();
                    _sceneSystem.OnProgressChanged += UpdateAmount;
                }
                background.gameObject.SetActive(true);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void UpdateAmount(float val)
        {
            loading.fillAmount = val;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}