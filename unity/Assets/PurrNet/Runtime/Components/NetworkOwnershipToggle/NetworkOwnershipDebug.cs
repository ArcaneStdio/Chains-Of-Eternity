namespace PurrNet
{
    public class NetworkOwnershipDebug : NetworkIdentity
    {
        [PurrButton]
        public void TakeOwnership()
        {
            GiveOwnership(localPlayer);
        }

        [PurrButton]
        public void ReleaseOwnership()
        {
            RemoveOwnership();
        }
    }
}
