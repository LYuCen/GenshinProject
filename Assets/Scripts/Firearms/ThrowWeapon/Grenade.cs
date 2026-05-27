using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using Firearms.Magazine;
using GameFSM.LevelFSM.PlayerFSM;
using mEnum.Flags;
using mEnum.Query;
using mEvent;
using mModel;
using mModel.ScriptableConfig;
using mSystem.BuffSystem.ElementBuff;
using mSystem.SoundSystem;
using QFramework;
using UnityEngine;

namespace Firearms.ThrowWeapon
{
    public enum GrenadeState { Idle, PullingPin, Ready, Throwing }
    public class Grenade : AbstractThrow, IWeapon, ICanSendEvent
    {
        public GrenadeState CurrentState = GrenadeState.Idle;
        public PlayerManager Manager { get; set; }
        public bool IsPullingPin => CurrentState == GrenadeState.PullingPin;
        public bool IsReady => CurrentState == GrenadeState.Ready;
        public bool IsThrowingAction => CurrentState == GrenadeState.Throwing;

        private LineRenderer _throwLine;
        private Vector3[] _points = new Vector3[50];
        private float _force = 10;
        private float _pointDistance = 0.02f;
        private Vector3 _yOffset = new Vector3(0, 0.04f, 0);
        private ThrowEvent _event;
        private WeaponGunConfig _config;
        private Dictionary<string, AnimationClip> _animDict = new();
        
        private bool _hasPhysicalThrown = false; // 物理投掷锁
        private bool _hasFinishedAction = false; // 动作结束锁

        private void ChangeGrenadeState(GrenadeState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
        
            // 重置这两个锁
            _hasPhysicalThrown = false; 
            _hasFinishedAction = false;

            //Manager.firearmController.currentActionNormalizedTime = 0f; 
            Debug.Log($"[Grenade] 进入阶段: {newState}");
        }

        // 动作 1：拉环结束
        public void OnPullPinFinish()
        {
            if (CurrentState == GrenadeState.PullingPin && !_hasFinishedAction)
            {
                _hasFinishedAction = true;
                ChangeGrenadeState(GrenadeState.Ready);
            }
        }

        // 动作 2：真正的物理投掷 (0.7时刻调用)
        public void ExecutePhysicalThrow()
        {
            if (CurrentState == GrenadeState.Throwing && !_hasPhysicalThrown)
            {
                _hasPhysicalThrown = true;
                ThrowGrenade(); // 实例化手雷
                Debug.Log("[Grenade] 物理手雷已投出！");
            }
        }

        // 动作 3：投掷动画完全结束 (0.95时刻调用)
        public void OnThrowAnimationEnd()
        {
            if (CurrentState == GrenadeState.Throwing && !_hasFinishedAction)
            {
                _hasFinishedAction = true;
                Debug.Log("[Grenade] 投掷动画播放完毕，回归 Idle");
                ChangeGrenadeState(GrenadeState.Idle);
            }
        }
        
        public void OnPrimaryAction()
        {
            if (!Enable || CurrentState != GrenadeState.Idle) return;
            ChangeGrenadeState(GrenadeState.PullingPin); 
        }

        public void UpPrimaryAction()
        {
            if (CurrentState == GrenadeState.Ready)
            {
                ChangeGrenadeState(GrenadeState.Throwing);
                _throwLine.enabled = false;
            }
        }

        public void PressPrimaryAction()
        {
            if (!Enable) return;

            // 如果处于 Ready 状态，显示投掷预案线
            if (CurrentState == GrenadeState.Ready)
            {
                _throwLine.enabled = true;
                UpdateTrajectory();
            }
        }

        private void UpdateTrajectory()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                _points[i] = Transform.position
                             + Forward * (_force * i * _pointDistance)
                             + Physics.gravity * (0.5f * i * _pointDistance * i * _pointDistance)
                             + _yOffset;
            }
            _throwLine.positionCount = _points.Length;
            _throwLine.SetPositions(_points);
        }


        public void OnSecondaryAction()
        {
            if (!Enable) return;
            Debug.Log("切换投掷模式");
            IsDownThrow = !IsDownThrow && CurrentState != GrenadeState.Idle;
            _force = IsDownThrow ? 5f : 10f; // 低抛力量为 5, 高抛力量为 10
        }

        public void PressSecondaryAction()
        {
            
        }

        public void UpSecondaryAction()
        {
            
        }

        public void ReloadAction(AbstractMagazine magazine)
        {
            Magazine = magazine;
            // 更新武器材质
            MeshRenderer meshRenderer = MagazineTransform.GetComponent<MeshRenderer>();
            Material[] materials = meshRenderer.materials;
            int m = materials.Length;
            materials[m - 1] = ElementModel.Instance.materials[(int)magazine.Element];
            meshRenderer.materials = materials;
        }

        public float ReloadTime()
        {
            return 0.0f;
        }

        public AbstractMagazine GetMagazine()
        {
            return Magazine;
        }

        public void UpdateForward(Vector3 forward)
        {
                // 投掷物仰角加 30 度
            float yAdd = Mathf.Sqrt(forward.x * forward.x + forward.z * forward.z) * 0.57735f;
            forward.y += yAdd;
            
            Forward = forward.normalized;
        }

        public void UpdateWeapon()
        {
            
        }

        public Transform GetWeapon()
        {
            if (!Transform)
            {
                IResource ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/Grenade/GrenadeWeapon.prefab", false);
                GameObject prefab = ir.GetAsset<GameObject>();
                Transform = Object.Instantiate(prefab).transform;
                _throwLine = Transform.GetComponent<LineRenderer>();
            }
            return Transform;
        }

        public void Init()
        {
            Magazine = new BaseMagazine(0, 1, 100, ElementEnum.Fire);
            MainRadius = 2f;
            OtherRadius = 5f;
            BoomCountDown = 1.5f;
            IsReadyThrow = false;
            IsThrow = false;
            IResource ir = ResourceManager.instance.Load("Assets/AssetBundle/ScriptableObject/Weapon/Grenade.asset", false);
            _config = ir.GetAsset<WeaponGunConfig>();
            
            foreach (var pair in _config.animMappings)
            {
                if (!_animDict.ContainsKey(pair.tag))
                    _animDict.Add(pair.tag, pair.clip);
            }
            
        }

        public void InitTransform()
        {
            if (!Transform) Transform = GetWeapon();
            _throwLine = Transform.gameObject.GetComponent<LineRenderer>();
            MagazineTransform = Transform.Find("Magazine");
            Transform.localPosition = new Vector3(0.000238f, 0.000363f, -0.00034f);
            Transform.localEulerAngles = new Vector3(27.714f, 174.281f, 182.487f);
            Transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
        }

        public void TryReload(int element)
        {
            
        }

        public void OnDisable()
        {
            
        }

        public void ThrowGrenade(bool haveForce = true)
        {
            GameObject g = GrenadeObjectPool.Instance.GetObj();
            
            Rigidbody rb = g.GetComponent<Rigidbody>();
            
            // 必须在设置位置和施加力之前，将速度和角速度彻底清零
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            GrenadeObj grenadeObj = g.GetComponent<GrenadeObj>();
            grenadeObj.Init(MainRadius, OtherRadius, BoomCountDown, (int)Magazine.Element, Magazine.BaseHurt);
            
            g.transform.position = Transform.position;
    
            // 施加新的力
            if(haveForce) 
            {
                Vector3 throwDir = Forward.magnitude > 0.1f ? Forward : Manager.transform.forward;
                rb.AddForce(throwDir * _force, ForceMode.VelocityChange);
            }
        }
        
        

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
        
        public void SetEnable(bool enable)
        {
            Enable = enable;
        }

        public bool GetEnable()
        {
            return Enable;
        }
        
        public void OnWeaponSwitchEnter()
        {
            AudioSystem.Instance.PlaySound(_config.getWeaponAudioClip);
        }

        public void OnWeaponSwitchExit()
        {
            
        }

        public WeaponGunConfig GetWeaponConfig()
        {
            return _config;
        }

        public AnimationClip GetClipByTag(string tag)
        {
            if (_animDict.TryGetValue(tag, out var clip))
            {
                return clip;
            }
        
            Debug.LogWarning($"武器 {_config.weaponName} 未配置 Tag 为 {tag} 的动画");
            return null;
        }

        public WeaponQuery GetQuery()
        {
            WeaponQuery query = new WeaponQuery();
            return query;
        }
    }
}