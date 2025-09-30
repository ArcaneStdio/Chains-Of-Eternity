using UnityEngine;

public class ResetSpell : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SpellObject firstspell;
    public ProjectileData originalprojectileData;
    public GameObject visualPrefab;

    public void ReturnToOriginal()
    {
        firstspell.attackData.projectileData = originalprojectileData;
        firstspell.visualPrefab = visualPrefab;
        Debug.Log("Spell Reset to Original");
    }
        private void OnDestroy()
    {
        Debug.Log("Scene is exiting! Running cleanup...");
        ReturnToOriginal();
    }
}
