using System;
using PurrNet.Packing;

namespace PurrNet.Modules
{
    public readonly struct SpawnID : IEquatable<SpawnID>, IPackedAuto
    {
        readonly PackedULong packetIdx;
        public readonly PlayerID player;

        public SpawnID(PackedULong packetIdx, PlayerID player)
        {
            this.packetIdx = packetIdx;
            this.player = player;
        }

        public bool Equals(SpawnID other)
        {
            return packetIdx == other.packetIdx && player.Equals(other.player);
        }

        public override bool Equals(object obj)
        {
            return obj is SpawnID other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(packetIdx, player);
        }

        public override string ToString()
        {
            return $"SpawnID: {{ packetIdx: {packetIdx}, player: {player} }}";
        }
    }
}
