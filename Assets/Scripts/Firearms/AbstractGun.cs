using System;
using System.Collections.Generic;
using Firearms.WeaponInterface;
using GameFSM.FSM;
using mModel;
using mModel.ScriptableConfig;
using mSystem.BuffSystem.ElementBuff;
using mSystem.SoundSystem;
using UnityEngine;

namespace Firearms
{
    /// <summary>
    /// 枪械抽象类
    /// </summary>
    public abstract class AbstractGun : IReloadable,IFsmDebugger
    {
        public AbstractMagazine Magazine; // 弹匣
        protected Transform Gun; // 枪
        protected Transform Muzzle;
        protected Transform MagazineTransform;
        public Transform Hand;
        public Transform Camera;
        protected Vector3 GunForward;
        protected WeaponGunConfig Config;
        public bool IsReload; // 是否正在换弹
        public bool IsShoot;
        public bool IsBolt;
        protected bool Enable = true;

        private Action _onActionComplete;
        protected BaseFsm<AbstractGun> Fsm;
        protected string UniqueId;
        
        // FSM 可视化
        public string GetCurrentStateName() => Fsm?.GetCurrentStateName();
        public IEnumerable<string> GetAllStateNames() => Fsm?.GetAllStateNames();
        public IEnumerable<string> GetConditions(string stateName) => Fsm?.GetConditions(stateName);
        // end

        protected void UpdateState()
        {
            Fsm.Update();
        }

        
        public abstract void Shoot();

        public virtual void RemoteShoot(byte[] flatData) { }

        public WeaponGunConfig GetWeaponConfig()
        {
            return Config;
        }

        public bool TryReload(ElementEnum ele, int bullet)
        {
            int index = (int)ele;
            // 子弹已上满并且上的是同元素的子弹
            if (Magazine.IsFull() && Magazine.GetElementEnum() == index) return false;
            // 子弹数量为 0
            if (bullet <= 0) return false;
            
            IsReload = true;
            IsShoot = false;
            return true;
        }
        
        public bool TryReload(AbstractMagazine magazine)
        {
            if (magazine == null) return false;
            
            IsReload = true;
            
            return true;
        }
        
        public void ReloadEnd(ElementEnum ele)
        {
            IsReload = false;
            // 更新武器材质
            if (MagazineTransform)
            {
                MeshRenderer meshRenderer = MagazineTransform.GetComponent<MeshRenderer>();
                Material[] materials = meshRenderer.materials;
                int m = materials.Length;
                materials[m - 1] = ElementModel.Instance.materials[Magazine.GetElementEnum()];
                meshRenderer.materials = materials;
            }
        }
        
        public void ReloadEnd(AbstractMagazine magazine)
        {
            Magazine = magazine;
            
            // 更新武器材质
            if (MagazineTransform)
            {
                MeshRenderer meshRenderer = MagazineTransform.GetComponent<MeshRenderer>();
                Material[] materials = meshRenderer.materials;
                int m = materials.Length;
                materials[m - 1] = ElementModel.Instance.materials[Magazine.GetElementEnum()];
                meshRenderer.materials = materials;
            }
        }
        
        public void StartControlledAction(Action onComplete)
        {
            _onActionComplete = onComplete;
        }

        /// <summary>
        /// 状态机结束后调用
        /// </summary>
        public void ExecuteActionSuccess()
        {
            _onActionComplete?.Invoke();
            _onActionComplete = null;
        }

        /// <summary>
        /// 被打断时调用
        /// </summary>
        public void CancelAction()
        {
            Debug.Log("动作被打断，回调已销毁");
            _onActionComplete = null; // 直接丢弃，这样逻辑永远不会被执行
        }

        public Transform GetMuzzle() => Muzzle;
        
        public void Dispose()
        {
            FsmDebuggerRegistry.Unregister(UniqueId);
        }
    }
}
