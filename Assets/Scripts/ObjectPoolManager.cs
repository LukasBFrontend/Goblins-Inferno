using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    readonly Dictionary<string, List<IPoolable>> _poolableObjectLookup = new();

    public T Spawn<T>(PoolableData data, Vector3 position, Quaternion rotation) where T : MonoBehaviour, IPoolable
    {
        T result;

        if (!_poolableObjectLookup.TryGetValue(data.id, out var pool))
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

        result.Id = data.id;
        result.gameObject.SetActive(true);

        return result;
    }

    public void Return<T>(T poolable) where T : MonoBehaviour, IPoolable
    {
        poolable.Reset();
        poolable.gameObject.SetActive(false);

        _poolableObjectLookup[poolable.Id].Add(poolable);
    }
}

public interface IPoolable
{
    string Id { get; set; }
    void Reset();
}
