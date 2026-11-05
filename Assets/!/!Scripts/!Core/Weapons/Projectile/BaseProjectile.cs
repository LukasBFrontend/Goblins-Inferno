using System.Collections;
using System.Collections.Generic;
using Luscious.Audio;
using UnityEngine;

public abstract class BaseProjectile: MonoBehaviour, IPoolable
{
    [SerializeField] SO_ProjectileData data;
    [SerializeField] GameObject render;
    [Header("Optional")]
    [SerializeField] ProjectileTargetTracker targetTracker;
    [SerializeField] ParticleSystemVariables particleSystem;
    [Header("Physics")]
    [SerializeField] Rigidbody rigidbody;

    private BaseCharacter _sender;
    private int _damage;
    private int _critChance;
    private List<BaseCharacter> _charactersHit = new();
    private SoundFXLifetimeData _aliveSound;

    protected ProjectileTargetTracker TargetTracker => targetTracker;
    protected Rigidbody Rigidbody => rigidbody;
    public SO_ProjectileData Data => data;
    protected IReadOnlyList<BaseCharacter> CharactersHit => _charactersHit;
    protected ParticleSystemVariables ParticleSystem => particleSystem;
    protected BaseCharacter Sender => _sender;

    public string PoolableId { get; set; }

    public void Initialize(BaseCharacter sender, BaseCharacter initialTarget, int damage, int critChance = 0)
    {
        _sender = sender;
        _damage = damage;
        _critChance = critChance;

        StartCoroutine(ProjectileRoutine(initialTarget));

        if (data.alive.audioClip != null)
        {
            _aliveSound = Game.SoundFX.PlaySoundFXClip(data.alive, transform.position, false, transform);
        }
    }

    public void Reset()
    {
        transform.rotation = Quaternion.identity;

        _sender = null;
        _damage = 0;
        _charactersHit.Clear();

        if (rigidbody)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        if (particleSystem.controls)
        {
            particleSystem.controls.Stop();
        }

        if (render)
        {
            render.SetActive(true);
        }
    }

    protected Vector3 PointToPosition(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z), 0);

        return direction;
    }

    public static Vector3 RandomMaxRange(Transform transform, float range)
    {
        Vector2 random = Random.insideUnitCircle.normalized;
        return transform.position + new Vector3(random.x, 0f, random.y) * range;
    }

    protected void RegisterHit(BaseCharacter character)
    {
        int damage = _damage * WeaponsUtils.RollCritModifier(_critChance, out int critLevel);
        character.Health.TakeDamage(damage, critLevel);
        _charactersHit.Add(character);
    }

    void OnEnable()
    {
        if (data.alive.audioClip == null)
        {
            return;
        }

        GameEvents.GamePaused.AddListener(PauseLiveSound);
        GameEvents.GameResumed.AddListener(PlayAliveSound);
        GameEvents.LvlUpEvent.AddListener((oldLevel, newLevel) => PauseLiveSound());
    }

    void OnDisable()
    {
        if(_aliveSound == null || !_aliveSound.Active)
        {
            return;
        }

        GameEvents.GamePaused.RemoveListener(PauseLiveSound);
        GameEvents.GameResumed.RemoveListener(PlayAliveSound);
        GameEvents.LvlUpEvent.RemoveListener((oldLevel, newLevel) => PauseLiveSound());

        _aliveSound.Destroy();
    }

    private void PlayAliveSound()
    {
        if(_aliveSound == null || !_aliveSound.Active)
        {
            return;
        }

        _aliveSound.AudioSource.Play();
    }

    protected void PauseLiveSound()
    {
        if(_aliveSound == null || !_aliveSound.Active)
        {
            return;
        }

        _aliveSound.AudioSource.Pause();
    }

    protected void StopAliveSound()
    {
        if(_aliveSound == null || !_aliveSound.Active)
        {
            return;
        }

        _aliveSound.AudioSource.Stop();
    }

    public abstract IEnumerator ProjectileRoutine(BaseCharacter target);

    [System.Serializable]
    protected struct ParticleSystemVariables
    {
        public ParticleSystemControls controls;
        [Range(0f, 1f)]
        public float tryRegisterHitsAt;
        [Range(0f, 1f)]
        public float soundEffectAt;
    }
}
