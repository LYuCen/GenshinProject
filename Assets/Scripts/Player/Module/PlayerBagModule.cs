using mModel;
using mSystem.BuffSystem.ElementBuff;
using Player.Context;
using QFramework;

namespace Player.Module
{
    public class PlayerBagModule : IModule, IController
    {
        private PlayerContext _context;
        private BagModel _bagModel;

        public PlayerBagModule(PlayerContext context)
        {
            _context = context;
            _bagModel = this.GetModel<BagModel>();
        }
        
        public void Init()
        {
            _bagModel.SetBullet(0, ElementEnum.Fire, 999);
            _bagModel.SetBullet(1, ElementEnum.Elect, 999);
            _bagModel.SetBullet(2, ElementEnum.Grass, 999);
            _bagModel.SetBullet(3, ElementEnum.Water, 999);
            _bagModel.SetBullet(4, ElementEnum.Frost, 999);
        }

        public void OnUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public BulletInfo GetBulletInfo(int chose)
        {
            return _bagModel.GetBullet(chose);
        }

        /// <summary>
        /// 归还子弹
        /// </summary>
        /// <param name="info"></param>
        public void ReturnBullet(BulletInfo info)
        {
            _bagModel.ReturnBullet(info);
        }

        /// <summary>
        /// 扣除子弹
        /// </summary>
        public void DeductBullet(BulletInfo info)
        {
            _bagModel.DeductBullet(info);
        }

        public void SetChose(int val)
        {
            _bagModel.SetBulletIndex(val);
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}