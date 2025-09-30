using System.Collections.Generic;

[System.Serializable]
public class WeaponData
{
    public string name;
    public int damage;
    public float attackSpeed;
    public float criticalRate;
    public float criticalDamage;
    public WeaponData Clone()
    {
        return new WeaponData
        {
            name = this.name,
            damage = this.damage,
            attackSpeed = this.attackSpeed,
            criticalRate = this.criticalRate,
            criticalDamage = this.criticalDamage
        };
    }
}

[System.Serializable]
public class ArmourData
{
    public string name;
    public ArmourSlot slot; // Helmet, Chest, etc.
    public int maxHealth;
    public int defense;
    public float healthRegeneration;
    public List<int> resistances = new List<int>();
    public ArmourData Clone()
    {
        return new ArmourData
        {
            name = this.name,
            slot = this.slot,
            maxHealth = this.maxHealth,
            defense = this.defense,
            healthRegeneration = this.healthRegeneration,
            resistances = new List<int>(this.resistances)
        };
    }
}

[System.Serializable]
public class ConsumableData
{
    public string name;
    public int healthAffected;
    public int manaAffected;
    public int energyAffected;
    public float cooldown;
    public float duration;
    public ConsumableData Clone()
    {
        return new ConsumableData
        {
            name = this.name,
            healthAffected = this.healthAffected,
            manaAffected = this.manaAffected,
            energyAffected = this.energyAffected,
            cooldown = this.cooldown,
            duration = this.duration
        };
    }
}

[System.Serializable]
public class AccessoryData
{
    public string name;
    public int bonusEnergy;
    public int bonusMana;
    public float bonusManaRegen;
    public float bonusEnergyRegen;
    public AccessoryData Clone()
    {
        return new AccessoryData
        {
            name = this.name,
            bonusEnergy = this.bonusEnergy,
            bonusMana = this.bonusMana,
            bonusManaRegen = this.bonusManaRegen,
            bonusEnergyRegen = this.bonusEnergyRegen
        };
    }
}
[System.Serializable]
public class DefaultData
{
    public string name;
    public string notes; // Add generic metadata or purpose
    public DefaultData Clone()
    {
        return new DefaultData
        {
            name = this.name,
            notes = this.notes
        };
    }
}