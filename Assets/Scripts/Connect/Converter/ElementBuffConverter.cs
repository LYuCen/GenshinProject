using Connect.ElementBuffDTO;
using Connect.Protocol;
using Google.FlatBuffers;

namespace Connect.Converter
{
    public static class ElementBuffConverter
    {
        public static Message PackAttach(int targetId, int element, int level, int val, int capacity)
        {
            var builder = new FlatBufferBuilder(64);
            var offset = ElementAttachFlat.CreateElementAttachFlat(builder, targetId, element, level, val, capacity);
            builder.Finish(offset.Value);
            return new Message(MessageType.S2C_ElementAttach, builder.SizedByteArray());
        }

        public static Message PackDetach(int targetId, int element)
        {
            var builder = new FlatBufferBuilder(32);
            var offset = ElementDetachFlat.CreateElementDetachFlat(builder, targetId, element);
            builder.Finish(offset.Value);
            return new Message(MessageType.S2C_ElementDetach, builder.SizedByteArray());
        }

        public static Message PackStatusEffect(int targetId, int effectType, bool isAdd, int duration)
        {
            var builder = new FlatBufferBuilder(64);
            var offset = StatusEffectFlat.CreateStatusEffectFlat(builder, targetId, effectType, isAdd, duration);
            builder.Finish(offset.Value);
            return new Message(MessageType.S2C_StatusEffect, builder.SizedByteArray());
        }
    }
}
