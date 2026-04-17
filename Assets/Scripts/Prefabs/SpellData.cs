using UnityEngine;

[CreateAssetMenu(fileName = "SpellData", menuName = "Scriptable Objects/SpellData")]
public class SpellData : ScriptableObject
{
    public enum SpellType
    {
        ray,
        ball,
        buff,
        structure
    }
    public enum CastType
    {
        auto,
        semi,
        charged
    }


    [Header("Spell options")]
    [SerializeField] private SpellType spell_Type;
    [SerializeField] private CastType cast_Type;
    [SerializeField] private int ammoSpace = 1;
    [SerializeField] private int currentAmmo = 1;
    [SerializeField] private int MaxCharge = 5;
    [SerializeField] private int currentCharge = 0;
    [SerializeField] private float ChargeTimePerUnit = 0.5f;
    [SerializeField] private float reloadTime = 1;
    [SerializeField] private float velocity = 1f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float shootDelay = 1f;
    [SerializeField] private float spreadIntensity = 0.1f;
    [SerializeField] private bool producesLine = false;

    [Header("Spell particles")]
    [SerializeField] private GameObject? spawnPrefab;
    [SerializeField] private GameObject? producedParticle;
    [SerializeField] private GameObject? hitParticle;

    [Header("Spell sound")]
    [SerializeField] private AudioSource? spawnSound;
    [SerializeField] private AudioSource? airSound;
    [SerializeField] private AudioSource? hitSound;

    public bool canCast = true;
    public bool isCasting = false;


    Coroutine CastingSpellCoroutine;
    public SpellType spellType { get => spell_Type; set => spell_Type = value; }
    public CastType castType { get => cast_Type; set => cast_Type = value; }
    public int AmmoSpace { get => ammoSpace; set => ammoSpace = value; }
    public float Velocity { get => velocity; set => velocity = value; }
    public float LifeTime { get => lifeTime; set => lifeTime = value; }
    public float ShootDelay { get => shootDelay; set => shootDelay = value; }
    public bool ProducesLine { get => producesLine; set => producesLine = value; }
    public GameObject? SpawnPrefab { get => spawnPrefab; set => spawnPrefab = value; }
    public GameObject? ProducedParticle { get => producedParticle; set => producedParticle = value; }
    public GameObject? HitParticle { get => hitParticle; set => hitParticle = value; }
    public AudioSource? SpawnSound { get => spawnSound; set => spawnSound = value; }
    public AudioSource? AirSound { get => airSound; set => airSound = value; }
    public AudioSource? HitSound { get => hitSound; set => hitSound = value; }
    public float Damage { get => damage; set => damage = value; }
    public float SpreadIntensity { get => spreadIntensity; set => spreadIntensity = value; }
    public int CurrentAmmo { get => currentAmmo; set => currentAmmo = value; }
    public float ReloadTime { get => reloadTime; set => reloadTime = value; }
    public int MaxCharge1 { get => MaxCharge; set => MaxCharge = value; }
    public int CurrentCharge { get => currentCharge; set => currentCharge = value; }
    public float ChargeTimePerUnit1 { get => ChargeTimePerUnit; set => ChargeTimePerUnit = value; }
}
