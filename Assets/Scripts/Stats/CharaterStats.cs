using UnityEngine;

public class CharaterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major Stats")]
    public Stat strength; // 1 point increase damage by 1 and crit.power by 1%
    public Stat agility; // 1 point increase evasion by 1% and crit.chance by 1 %
    public Stat intelligence; // 1 point increase magic by 1 and magic resistance by 3
    public Stat vitality; // 1 point increase health by 3 or 5 points

    [Header("Offensive Stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;       // default value 150%

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited; // does damage over time
    public bool isChilled; // reduce armor by 20%
    public bool isShocked; // reduce accuracy by 20%

    [SerializeField] private float alimentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;


    private float igniteDamageCoolDown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;


    public int currentHealth;
    public System.Action onHealthChanged;
    public bool isDead { get; private set; }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;



        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (isIgnited)
            ApplyIgniteDamage();
    }

    

    public virtual void DoDamage(CharaterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
        //DoMagicDamage(_targetStats);
    }

    #region Magical damage and aliments

    public virtual void DoMagicDamage(CharaterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;
        bool canApplyIgnite, canApplyChill, canApplyShock;
        AttemptyToApplyAliements(_targetStats, _fireDamage, _iceDamage, _lightingDamage, out canApplyIgnite, out canApplyChill, out canApplyShock);

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);

    }

    private void AttemptyToApplyAliements(CharaterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage, out bool canApplyIgnite, out bool canApplyChill, out bool canApplyShock)
    {
        canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;
        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = alimentsDuration;

            fx.IgniteFxFor(alimentsDuration);
        }

        if (_chill && canApplyChill)
        {
            chilledTimer = alimentsDuration;
            isChilled = _chill;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, alimentsDuration);
            fx.ChillFxFor(alimentsDuration);
        }

        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;
                HitNearestTagetWithShockStrick();

            }

        }
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        shockedTimer = alimentsDuration;
        isShocked = _shock;
        fx.ShockFxFor(alimentsDuration);
    }

    private void HitNearestTagetWithShockStrick()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
        Transform closestEnemy = null;

        float closestDistance = Mathf.Infinity;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }

            }

            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrick = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrick.GetComponent<ShockStrikeController>().Setup(shockDamage, closestEnemy.GetComponent<CharaterStats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {

            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead)
                Die();

            igniteDamageTimer = igniteDamageCoolDown;
        }
    }
    
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion
    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth < 0 && !isDead)
            Die();

    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;
        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
    }
    #region Stat Calculation

    private int CheckTargetArmor(CharaterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }


    private int CheckTargetResistance(CharaterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }


    private bool TargetCanAvoidAttack(CharaterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }

        return false;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;


        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue();
    } 
    #endregion

}
