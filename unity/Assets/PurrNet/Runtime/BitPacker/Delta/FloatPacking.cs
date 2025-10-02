using PurrNet.Modules;

namespace PurrNet.Packing
{
    public static class FloatPacking
    {
        [UsedByIL]
        public static unsafe void Write(this BitPacker packer, float data)
        {
            ulong bits = *(uint*)&data;
            packer.WriteBits(bits, 32);
        }

        [UsedByIL]
        public static unsafe void Read(this BitPacker packer, ref float data)
        {
            ulong bits = packer.ReadBits(32);
            data = *(float*)&bits;
        }

        [UsedByIL]
        private static unsafe bool WriteSingle(BitPacker packer, float oldvalue, float newvalue)
        {
            uint newbits = *(uint*)&newvalue;
            uint oldbits = *(uint*)&oldvalue;

            if (newbits == oldbits)
            {
                Packer<bool>.Write(packer, false);
                return false;
            }

            Packer<bool>.Write(packer, true);

            // Extract components more efficiently
            uint newSign = newbits >> 31;
            uint oldSign = oldbits >> 31;

            int newExp = (int)((newbits >> 23) & 0xFF);
            int oldExp = (int)((oldbits >> 23) & 0xFF);

            int newMantissa = (int)(newbits & 0x7FFFFF);
            int oldMantissa = (int)(oldbits & 0x7FFFFF);

            // Pack sign change as single bit instead of full sign
            bool signChanged = newSign != oldSign;
            Packer<bool>.Write(packer, signChanged);

            Packer<PackedInt>.Write(packer, newExp - oldExp);
            Packer<PackedInt>.Write(packer, newMantissa - oldMantissa);

            return true;
        }

        [UsedByIL]
        private static unsafe void ReadSingle(BitPacker packer, float oldvalue, ref float value)
        {
            bool hasChanged = default;
            Packer<bool>.Read(packer, ref hasChanged);

            if (!hasChanged)
            {
                value = oldvalue;
                return;
            }

            uint oldbits = *(uint*)&oldvalue;

            bool signChanged = default;
            PackedInt expDiff = default;
            PackedInt mantissaDiff = default;

            Packer<bool>.Read(packer, ref signChanged);
            Packer<PackedInt>.Read(packer, ref expDiff);
            Packer<PackedInt>.Read(packer, ref mantissaDiff);

            // Reconstruct efficiently
            uint sign = (oldbits >> 31) ^ (signChanged ? 1u : 0u);
            uint exp = (uint)((int)((oldbits >> 23) & 0xFF) + expDiff.value) & 0xFF;
            uint mantissa = (uint)((int)(oldbits & 0x7FFFFF) + mantissaDiff.value) & 0x7FFFFF;

            uint bits = (sign << 31) | (exp << 23) | mantissa;
            value = *(float*)&bits;
        }
    }
}
