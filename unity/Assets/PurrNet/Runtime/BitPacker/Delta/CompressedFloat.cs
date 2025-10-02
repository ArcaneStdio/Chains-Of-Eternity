using System;
using System.Globalization;
using UnityEngine;

namespace PurrNet.Packing
{
    [System.Serializable]
    public struct CompressedFloat : IEquatable<CompressedFloat>
    {
        public const float PRECISION = 0.001f;

        public float value;

        public CompressedFloat(float value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static implicit operator CompressedFloat(float value) => new CompressedFloat(value);
        public static implicit operator float(CompressedFloat angle) => angle.value;

        public static implicit operator CompressedFloat(PackedInt value) => new CompressedFloat(value.value * PRECISION);
        public static implicit operator PackedInt(CompressedFloat angle) => new PackedInt(Mathf.RoundToInt(angle.value / PRECISION));

        public PackedInt ToPackedInt()
        {
            return Mathf.RoundToInt(value / PRECISION);
        }

        public CompressedFloat Round()
        {
            var copy = this;
            var rounded = Mathf.RoundToInt(value / PRECISION);
            copy.value = rounded * PRECISION;
            return copy;
        }

        public bool Equals(CompressedFloat other)
        {
            return (int)(value / PRECISION) == (int)(other.value / PRECISION);
        }

        public override bool Equals(object obj)
        {
            return obj is CompressedFloat other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
