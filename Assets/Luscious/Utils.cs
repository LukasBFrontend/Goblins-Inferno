namespace Luscious
{
    class Utils
    {
        public static T GetSingleton<T>() where T: Singleton<T>
        {
            return Singleton<T>.GetInstance();
        }
    }
}
