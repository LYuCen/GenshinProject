using Firearms.Magazine;
using mEvent;
using mModel;
using mSystem.BuffSystem.ElementBuff;
using mSystem.SoundSystem;
using QFramework;
using UnityEngine;

namespace Firearms.ThrowWeapon
{
    public class Grenade : AbstractThrow, IWeapon, ICanSendEvent
    {
        private LineRenderer _throwLine;
        private Vector3[] _points = new Vector3[100];
        private float _force;
        private float _pointDistance = 0.02f;
        private Vector3 _yOffset = new Vector3(0, 0.04f, 0);
        private ThrowEvent _event;

        private bool _isOnPrimaryAction; // 检查是否执行过 OnPrimaryAction
        public void OnPrimaryAction()
        {
            // 点击投掷时默认高抛
            if (!Enable) return;
            _isOnPrimaryAction = true;
            _force = 10f;
            _throwLine.enabled = true;
            SoundSystem.Instance.PlaySound("Assets/Resources/Sound/SoundEffect/Grenade/PullOut.mp3");
        }

        public void PressPrimaryAction()
        {
            if (!Enable) return;
            if(!_isOnPrimaryAction) OnPrimaryAction();
            IsReadyThrow = true;
            for (int i = 0; i < _points.Length; i++)
            {
                _points[i] = Transform.position + Forward * (_force * i * _pointDistance)
                                                + Physics.gravity * (0.5f * i * _pointDistance * i * _pointDistance)
                                                + _yOffset;
                
            }

            _throwLine.positionCount = _points.Length;
            _throwLine.SetPositions(_points);
        }

        public void UpPrimaryAction()
        {
            if (!Enable) return;
            IsDownThrow = false;
            IsReadyThrow = false;
            Enable = false;
            this.SendEvent(_event);
            ThrowGrenade();
            _throwLine.enabled = false;
            _isOnPrimaryAction = false;
            //Debug.Log("投掷手雷");
        }

        public void OnSecondaryAction()
        {
            if (!Enable) return;
            IsDownThrow = !IsDownThrow && IsReadyThrow;
            _force = IsDownThrow ? 5f : 10f; // 低抛力量为 5, 高抛力量为 10
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
            return Transform;
        }

        public void Init()
        {
            Transform = Object.Instantiate(Resources.Load<Transform>("Prefab/G"));
            _throwLine = Transform.gameObject.GetComponent<LineRenderer>();
            MagazineTransform = Transform.Find("Magazine");
            Magazine = new BaseMagazine(0, 1, 100, ElementEnum.Fire);
            MainRadius = 2f;
            OtherRadius = 5f;
            BoomCountDown = 3f;
            IsReadyThrow = false;
            IsThrow = false;
        }

        public void InitTransform()
        {
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

        private void ThrowGrenade()
        {
            GameObject g = GrenadeObjectPool.Instance.GetObj();
            GrenadeObj grenadeObj = g.GetComponent<GrenadeObj>();
            grenadeObj.Init(MainRadius, OtherRadius, BoomCountDown, (int)Magazine.Element, Magazine.BaseHurt);
            g.transform.position = Transform.position;
            // ForceMode.VelocityChange 忽略物体密度，防止影响力度对物体移动的控制
            g.GetComponent<Rigidbody>().AddForce(Forward * _force, ForceMode.VelocityChange);
            
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
            
        }

        public void OnWeaponSwitchExit()
        {
            
        }
    }
}