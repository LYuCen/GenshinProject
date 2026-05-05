using GameFSM.LevelFSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;

namespace Firearms
{
    /// <summary>
    /// 武器基础信息
    /// </summary>
    public interface IWeapon
    {
        public PlayerManager Manager { get; set; }

        /// <summary>
        /// 按下主功能键 （默认左键）
        /// </summary>
        void OnPrimaryAction();
        
        /// <summary>
        /// 长按主功能键
        /// </summary>
        void PressPrimaryAction();
        
        /// <summary>
        /// 抬起主功能键
        /// </summary>
        void UpPrimaryAction();
        
        /// <summary>
        /// 按下副功能键 （默认右键）
        /// </summary>
        void OnSecondaryAction(ref bool isShoot);

        /// <summary>
        /// 换弹剩余时间
        /// </summary>
        /// <returns></returns>
        float ReloadTime();

        /// <summary>
        /// 获得当前弹匣
        /// </summary>
        AbstractMagazine GetMagazine();
        

        /// <summary>
        /// 更新武器正朝向
        /// </summary>
        void UpdateForward(Vector3 forward);

        /// <summary>
        /// 更新武器信息
        /// </summary>
        void UpdateWeapon();

        /// <summary>
        /// 获得武器预制体
        /// </summary>
        /// <returns>武器 Transform</returns>
        Transform GetWeapon();

        /// <summary>
        /// 初始化
        /// </summary>
        void Init();

        /// <summary>
        /// 初始化 Transform 位置旋转等信息
        /// </summary>
        void InitTransform();

        /// <summary>
        /// 修改武器功能激活状态
        /// </summary>
        /// <param name="enable">是否激活</param>
        void SetEnable(bool enable);
        
        /// <summary>
        /// 获取武器功能是否激活
        /// </summary>
        /// <returns></returns>
        bool GetEnable();
        
        /// <summary>
        /// 切换到该武器时调用
        /// </summary>
        void OnWeaponSwitchEnter();
        
        /// <summary>
        /// 切换到其他武器时调用
        /// </summary>
        void OnWeaponSwitchExit();
        
        /// <summary>
        /// 获取武器信息
        /// </summary>
        /// <returns></returns>
        WeaponGunConfig GetWeaponConfig();
        
        /// <summary>
        /// 通过标签查询特殊武器动画
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        AnimationClip GetClipByTag(string tag);
    }
}