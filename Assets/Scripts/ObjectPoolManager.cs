using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    [Space()]
    [SerializeField] Transform poolParent;
    [SerializeField] List<PoolPrewarmData> poolPrewarmData;
    readonly Dictionary<string, List<IPoolable>> _poolableObjectLookup = new();
    readonly Dictionary<string, Transform> _transformLookup = new();

    public T Spawn<T>(PoolableData data, Vector3 position, Quaternion rotation) where T : MonoBehaviour, IPoolable
    {
        T result;

        if (!_poolableObjectLookup.TryGetValue(data.id, out List<IPoolable> pool))
        {
            pool = CreatePool(data.id);
        }

        if (pool.Count > 0)
        {
            result = (T)pool[0];
            pool.RemoveAt(0);

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

        result.PoolableId = data.id;
        result.gameObject.SetActive(true);
        result.transform.SetParent(_transformLookup[data.id]);

        return result;
    }

    public void Return<T>(T poolable) where T : MonoBehaviour, IPoolable
    {
        poolable.Reset();
        poolable.gameObject.SetActive(false);

        _poolableObjectLookup[poolable.PoolableId].Add(poolable);
    }

    protected override void OnSingletonAwake()
    {
        foreach(var prewarmData in poolPrewarmData)
        {
            for (int i = 0; i < prewarmData.spawnAmount; i++)
            {
                Prewarm(prewarmData.poolable);
            }
        }
    }

    List<IPoolable> CreatePool(string poolableId)
    {
            List<IPoolable> pool = new();
            _poolableObjectLookup.Add(poolableId, pool);

            GameObject gameObject = new()
            {
                name = $"{poolableId} Pool"
            };

            gameObject.transform.SetParent(poolParent);
            _transformLookup.Add(poolableId, gameObject.transform);

            return pool;
    }

    void Prewarm(PoolableData data)
    {

        if (!_poolableObjectLookup.TryGetValue(data.id, out List<IPoolable> pool))
        {
            pool = CreatePool(data.id);
        }

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

        pool.Add(poolable);
    }

    IEnumerator DisableNextFrameRoutine(GameObject gameObject)
    {
        yield return new WaitForSeconds(.1f);
        gameObject.SetActive(false);
    }
}

public interface IPoolable
{
    string PoolableId { get; set; }
    void Reset();
}

[System.Serializable] struct PoolPrewarmData
{
    public PoolableData poolable;
    public int spawnAmount;
}
