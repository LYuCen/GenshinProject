using System.Collections.Generic;
using mModel;
using mSystem.BuffSystem.ElementBuff;
using mWeapon;

namespace Server
{
    public class ServerInventory : IInventory
    {
        private List<BulletInfo> _bullets = new();
        private int _index;

        public ElementEnum ChosenElement => _bullets[_index].Data;
        public BulletInfo GetBullet(ElementEnum e)
        {
            for (int i = 0; i < _bullets.Count; i++)
            {
                if (_bullets[i].Data == e) return _bullets[i];
            }

            return new BulletInfo();
        }

        public void ReturnBullet(BulletInfo info)
        {
            for (int i = 0; i < _bullets.Count; i++)
            {
                if (_bullets[i].Data == info.Data)
                {
                    var b = _bullets[i];
                    b.Val += info.Val;
                    _bullets[i] = b;
                }
            }
        }

        public void DeductBullet(BulletInfo info)
        {
            for (int i = 0; i < _bullets.Count; i++)
            {
                if (_bullets[i].Data == info.Data)
                {
                    var b = _bullets[i];
                    b.Val -= info.Val;
                    _bullets[i] = b;
                }
            }
        }

        public void Seed(ElementEnum e, int val)
        {
            _bullets.Add(new BulletInfo(e, val));
        }

        public void SetChose(int index)
        {
            if (index >= 0 && index < _bullets.Count) _index = index;
        }
    }
}