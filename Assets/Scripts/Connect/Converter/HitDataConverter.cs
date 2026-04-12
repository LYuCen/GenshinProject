using Google.FlatBuffers;
using Connect.DTO;  
using Connect.HitDTO;

namespace Connect.Converter
{
    public static class HitDataConverter
    {
        // 复用 Builder 减少内存碎片
        private static FlatBufferBuilder _hitBuilder = new FlatBufferBuilder(256);

        // 1. 打包：HitDto -> byte[]
        public static byte[] ToBinary(HitDto data)
        {
            _hitBuilder.Clear();

            // 开始构建
            HitDataFlat.StartHitDataFlat(_hitBuilder);
            
            HitDataFlat.AddAttackerId(_hitBuilder, data.attackerId);
            HitDataFlat.AddVictimId(_hitBuilder, data.victimId);
            HitDataFlat.AddDamage(_hitBuilder, data.damage);
            HitDataFlat.AddHitType(_hitBuilder, data.hitType);
            HitDataFlat.AddElement(_hitBuilder, data.element);
            HitDataFlat.AddElementMeasure(_hitBuilder, data.elementMeasure);

            var offset = HitDataFlat.EndHitDataFlat(_hitBuilder);
            _hitBuilder.Finish(offset.Value);

            return _hitBuilder.SizedByteArray();
        }

        // 2. 解包：byte[] -> 填充到 HitDto
        public static void FillData(byte[] buffer, HitDto data)
        {
            var bb = new ByteBuffer(buffer);
            var flat = HitDataFlat.GetRootAsHitDataFlat(bb);

            data.attackerId = flat.AttackerId;
            data.victimId = flat.VictimId;
            data.damage = flat.Damage;
            data.hitType = flat.HitType;
            data.element = flat.Element;
            data.elementMeasure = flat.ElementMeasure;
        }
    }
}