using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[System.Serializable]
public class SpellResponse
{
    public string id;
    public string spell_name;
    public string element;
    public float mana_cost;
    public float cooldown;
    public string attack_subtype;

    // Combat
    public int damage;
    public float knockback_force;

    // Projectile-specific
    public string movement_path;
    public float projectile_speed;
    public float projectile_size;
    public int number_of_projectiles;
    public float delay_between_projectiles;
    public float staggered_launch_angle;

    public List<Vector2> spawn_offsets;
    public List<Vector2> directions;

    // Extra configs
    public float zigzag_amplitude;
    public float zigzag_frequency;
    public float homing_delay;
    public float homing_radius;
    public float homing_update_rate;
    public float circular_initial_radius;
    public float circular_speed;
    public float circular_radial_speed;
    public float random_direction_offset;
    public float arc_gravity_scale;

    public string created_at;
    public string created_by;
}

[System.Serializable]
public class SpellResponseList
{
    public List<SpellResponse> spells;
}

public class SpellUpdateSystem : MonoBehaviour
{
    [Header("Assign Spells in Inspector")]
    public SpellObject FirstSpell;
    public SpellObject SecondSpell;
    public SpellObject ThirdSpell;
    public SpellObject FourthSpell;

    [Header("Assign Projectiles in Inspector")]
    public ProjectileData FirstProjectileData;
    public ProjectileData SecondProjectileData;
    public ProjectileData ThirdProjectileData;
    public ProjectileData FourthProjectileData;

    private string url = "http://localhost:3000/user-spell";

    void Start()
    {
        StartCoroutine(LoadSpells("0x0095f13a82f1a834")); // pass user address here
    }

    IEnumerator LoadSpells(string address)
    {
        // prepare body
        SpellRequest requestData = new SpellRequest { address = address };
        string jsonData = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        //Debug.Log(bodyRaw);
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawJson = request.downloadHandler.text;

                // Wrap array properly
                Debug.Log("Raw JSON response: " + rawJson);
                string wrapped = "{\"spells\":" + rawJson + "}";
                SpellResponseList spellList = JsonUtility.FromJson<SpellResponseList>(wrapped);

                Debug.Log("Spell loading response: " + (spellList.spells == null ? "null" : spellList.spells.Count.ToString()));
                Debug.Log("SpellList: " + spellList);
                
                if (spellList != null && spellList.spells != null && spellList.spells.Count > 0)
                {
                    Debug.Log("----------");
                    Debug.Log($"Fetched {spellList.spells.Count} spells from API.");
                    Debug.Log("----------");
                    if (spellList.spells.Count > 0) FillSpell(spellList.spells[0], FirstSpell, FirstProjectileData);
                    if (spellList.spells.Count > 1) FillSpell(spellList.spells[1], SecondSpell, SecondProjectileData);
                    if (spellList.spells.Count > 2) FillSpell(spellList.spells[2], ThirdSpell, ThirdProjectileData);
                    if (spellList.spells.Count > 3) FillSpell(spellList.spells[3], FourthSpell, FourthProjectileData);
                }
                else
                {
                    Debug.LogWarning("No spells returned from API.");
                }
            }
            else
            {
                Debug.LogError("Failed to fetch spells: " + request.error);
            }
        }
        Debug.Log("Spell loading process completed.");
    }

    private void FillSpell(SpellResponse spell, SpellObject spellObj, ProjectileData projData)
    {
        //if (spellObj == null || projData == null) return;

        // Map to SpellObject
        spellObj.spellName = spell.spell_name;
        spellObj.element = ParseElement(spell.element);
        spellObj.manaCost = spell.mana_cost;
        spellObj.cooldown = spell.cooldown;
        spellObj.attackSubtype = ParseAttackSubtype(spell.attack_subtype);

        // Map to ProjectileData
        projData.damage = spell.damage;
        projData.knockbackForce = spell.knockback_force;
        projData.projectileSpeed = spell.projectile_speed;
        projData.projectileSize = spell.projectile_size;
        projData.numberOfProjectiles = spell.number_of_projectiles;
        projData.delayBetweenProjectiles = spell.delay_between_projectiles;
        projData.staggeredLaunchAngle = spell.staggered_launch_angle;

        projData.spawnOffsets = spell.spawn_offsets ?? new List<Vector2> { Vector2.zero };
        projData.directions = spell.directions ?? new List<Vector2> { Vector2.right };

        projData.zigzagAmplitude = spell.zigzag_amplitude;
        projData.zigzagFrequency = spell.zigzag_frequency;
        projData.homingDelay = spell.homing_delay;
        projData.homingRadius = spell.homing_radius;
        projData.homingUpdateRate = spell.homing_update_rate;
        projData.circularInitialRadius = spell.circular_initial_radius;
        projData.circularSpeed = spell.circular_speed;
        projData.circularRadialSpeed = spell.circular_radial_speed;
        projData.randomDirectionOffset = spell.random_direction_offset;
        projData.arcGravityScale = spell.arc_gravity_scale;

        Debug.Log($"Filled SpellObject: {spellObj.spellName} ({spellObj.element}, {spellObj.attackSubtype})");
    }

    private ElementType ParseElement(string element)
    {
        return element switch
        {
            "Fire" => ElementType.Fire,
            "Water" => ElementType.Water,
            "Lightning" => ElementType.Lightning,
            "Wind" => ElementType.Wind,
            _ => ElementType.Fire
        };
    }

    private AttackSubtype ParseAttackSubtype(string subtype)
    {
        return subtype switch
        {
            "Projectile" => AttackSubtype.Projectile,
            "AoE" => AttackSubtype.AoE,
            _ => AttackSubtype.Projectile
        };
    }
}


[System.Serializable]
public class SpellRequest : MonoBehaviour
{
    public string address;
}