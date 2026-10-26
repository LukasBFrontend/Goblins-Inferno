using UnityEngine;


namespace Luscious
{
    /// <summary>
    /// Applies a singleton pattern to a MonoBehaviour of type T.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] SingletonData singleton;
        private static T instance;

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;
            if (singleton.dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this);
            }

            OnSingletonAwake();
        }

        /// <summary>
        /// Replaces the Awake method for Monobehaviors which inherit from this class.
        /// </summary>
        protected virtual void OnSingletonAwake(){}

        public static T GetInstance()
        {
            return instance;
        }

        [System.Serializable]
        public struct SingletonData
        {
            public bool dontDestroyOnLoad;
        }
    }
}
