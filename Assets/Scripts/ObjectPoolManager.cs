using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    [SerializeField] List<PoolPrewarmData> poolPrewarmData;
    readonly Dictionary<string, List<IPoolable>> _poolableObjectLookup = new();

    public T Spawn<T>(PoolableData data, Vector3 position, Quaternion rotation) where T : MonoBehaviour, IPoolable
    {
        T result;

        if (!_poolableObjectLookup.TryGetValue(data.id, out List<IPoolable> pool))
        {
            pool = new List<IPoolable>();
            _poolableObjectLookup.Add(data.id, pool);
        }

        if (pool.Count > 0)
        {
            result = (T)pool[0];
            pool.RemoveAt(0);

            result.gameObject.transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            GameObject obj = Instantiate(data.prefab, position, rotation);
            result = obj.GetComponent<T>();

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

    void Prewarm(PoolableData data)
    {
        GameObject gameObject = Instantiate(
            data.prefab,
            new (0, 2000, 0),
            Quaternion.identity
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

        if (!_poolableObjectLookup.TryGetValue(poolable.PoolableId, out List<IPoolable> pool))
        {
            pool = new List<IPoolable>();
            _poolableObjectLookup.Add(poolable.PoolableId, pool);
        }

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
