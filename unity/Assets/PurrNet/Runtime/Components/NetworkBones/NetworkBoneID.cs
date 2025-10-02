using PurrNet.Modules;

namespace PurrNet
{
    public readonly struct NetworkBoneID : IStableHashable
    {
        readonly uint hash;

        public NetworkBoneID(SceneID sceneId, NetworkID id, int index, BoneInfoType type)
        {
            int typeHash = (int)type;
            hash = (uint)(sceneId.id.value ^ (uint)id.id.value ^ (uint)id.scope.id.value ^ index ^ typeHash);
        }

        public uint GetStableHash()
        {
            return hash;
        }
    }
}
