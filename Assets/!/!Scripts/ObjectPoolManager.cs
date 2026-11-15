using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Luscious;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    [Space()]
    [SerializeField] Transform poolParent;
    [SerializeField] List<PoolPrewarmData> poolPrewarmData;

    private readonly Dictionary<string, PoolGroup> _poolableObjectLookup = new();
    private readonly Dictionary<string, Transform> _transformLookup = new();

    public T Spawn<T>(PoolableData data, Vector3 position, Quaternion rotation) where T : MonoBehaviour, IPoolable
    {
        T result;

        if (!_poolableObjectLookup.TryGetValue(data.id, out PoolGroup group))
        {
            group = CreatePoolGroup(data.id);
        }

        if (group.pool.Count > 0)
        {
            result = (T)group.pool.Dequeue();

            result.gameObject.transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            GameObject gameObject = Instantiate(data.prefab, position, rotation);
            result = gameObject.GetComponent<T>();

            if (result == null)
            {
                Debug.LogError(
                    $"Prefab <color=white>'{data.prefab.name}'</color> does not implement IPoolable."
                );
                return null;
            }
        }

        if (group.isPrewarmRequired && group.pool.Count < 20)
        {
            Prewarm(data, 10);
        }

        result.PoolableId = data.id;
        result.gameObject.SetActive(true);
        result.transform.SetParent(_transformLookup[data.id]);

        return result;
    }

    public void Return<T>(T poolable) where T : MonoBehaviour, IPoolable
    {
        poolable.Reset();
        poolable.gameObject.SetActive(false);

        _poolableObjectLookup[poolable.PoolableId].pool.Enqueue(poolable);
    }

    protected override void OnSingletonAwake()
    {
        foreach(var prewarmData in poolPrewarmData)
        {
            Prewarm(prewarmData.poolable, prewarmData.spawnAmount);
        }
    }

    private PoolGroup CreatePoolGroup(string poolableId, bool isPrewarmRequired = false)
    {
            PoolGroup poolGroup = new(isPrewarmRequired);
            _poolableObjectLookup.Add(poolableId, poolGroup);

            GameObject gameObject = new()
            {
                name = $"{poolableId} Pool"
            };

            gameObject.transform.SetParent(poolParent);
            _transformLookup.Add(poolableId, gameObject.transform);

            return poolGroup;
    }

    private void Prewarm(PoolableData data, int amount)
    {
        if (!_poolableObjectLookup.TryGetValue(data.id, out PoolGroup poolGroup))
        {
            poolGroup = CreatePoolGroup(data.id, true);
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject gameObject = Instantiate(
                data.prefab,
                new (0, 2000, 0),
                Quaternion.identity,
                _transformLookup[data.id]
            );

            bool isPoolable = gameObject.TryGetComponent<IPoolable>(out var poolable);

            if (!isPoolable)
            {
                Debug.LogError(
                    $"Prefab <color=white>'{data.prefab.name}'</color> does not implement IPoolable."
                );

                Destroy(gameObject);
                return;
            }

            poolable.PoolableId = data.id;
            StartCoroutine(DisableNextFrameRoutine(gameObject)); // Give time for initialization.

            poolGroup.pool.Enqueue(poolable);
        }
    }

    private IEnumerator DisableNextFrameRoutine(GameObject gameObject)
    {
        yield return new WaitForSeconds(.1f);
        gameObject.SetActive(false);
    }


    [System.Serializable]
    private struct PoolPrewarmData
    {
        public PoolableData poolable;
        public int spawnAmount;
    }

    private struct PoolGroup
    {
        public bool isPrewarmRequired;
        public Queue<IPoolable> pool;

        public PoolGroup(bool isPrewarmRequired)
        {
            this.isPrewarmRequired = isPrewarmRequired;
            pool = new Queue<IPoolable>();
        }
    }
}

public interface IPoolable
{
    string PoolableId { get; set; }
    void Reset();
}


[System.Serializable]
public struct PoolableData
{
    public string id;
    public GameObject prefab;

    public PoolableData(string id, GameObject prefab)
    {
        this.id = id;
        this.prefab = prefab;
    }
}
