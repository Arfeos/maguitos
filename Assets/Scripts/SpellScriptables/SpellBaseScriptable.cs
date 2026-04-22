using UnityEngine;
using static SpellBase;

[CreateAssetMenu(fileName = "SpellBaseScriptable", menuName = "Scriptable Objects/SpellBaseScriptable")]
public class SpellBaseScriptable : ScriptableObject
{

    [Header("Spell options")]
    public SpellType spell_Type;
    public CastType cast_Type;
    public Spellimportance spell_importance;
    public int ammoSpace = 1;
    public int currentAmmo = 1;
    public int MaxCharge = 5;
    public int currentCharge = 0;
    public float ChargeTimePerUnit = 0.5f;
    public float reloadTime = 1;
    public float velocity = 1f;
    public float damage = 1f;
    public float lifeTime = 1f;
    public float shootDelay = 1f;
    public float spreadIntensity = 0.1f;
    public bool producesLine = false;
    public int CosteSlots = 1;
    public string nombreHechizo = "Change Spell name";

    [Header("Spell particles")]
     public GameObject? spawnPrefab;
     public GameObject? producedParticle;
     public GameObject? hitParticle;

    [Header("Spell sound")]
    public AudioSource? spawnSound;
    public AudioSource? airSound;
    public AudioSource? hitSound;


    public bool canCast = true;
    public bool isCasting = false;
}
