using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    readonly Dictionary<string, List<IPoolable>> _poolableObjectLookup = new();

    public IPoolable Spawn(PoolableData data, Vector3 position, Quaternion rotation)
    {
        if (!_poolableObjectLookup.TryGetValue(data.id, out var pool))
        {
            pool = new List<IPoolable>();
            _poolableObjectLookup.Add(data.id, pool);
        }

        IPoolable poolable;

        if (pool.Count > 0)
        {
            poolable = pool[0];
            pool.RemoveAt(0);

            poolable.SetPosition(position);
            poolable.GameObject.transform.rotation = rotation;
        }
        else
        {
            GameObject obj = Instantiate(data.prefab, position, rotation);
            poolable = obj.GetComponent<IPoolable>();

            if (poolable == null)
            {
                Debug.LogError(
                    $"Prefab '{data.prefab.name}' does not implement IPoolable."
                );
                return null;
            }
        }

        poolable.PoolId = data.id;
        poolable.GameObject.SetActive(true);

        return poolable;
    }

    public void Return(IPoolable poolable)
    {
        poolable.Reset();
        poolable.GameObject.SetActive(false);

        _poolableObjectLookup[poolable.PoolId].Add(poolable);
    }
}

public interface IPoolable
{
    string PoolId { get; set; }
    GameObject GameObject { get; }
    void SetPosition(Vector3 newPosition);
    void ReturnToPool();
    void Reset();
}
