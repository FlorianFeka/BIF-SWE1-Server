using System;
using System.Collections.Generic;

namespace MonsterTradingCardsGame.Util
{
    // Really just here so the Database Connection is continues and only is closed with the application
    public static class SingletonFactory
    {
        public static Dictionary<Type, object> singetons = new Dictionary<Type, object>();

        public static T GetObject<T>()
        {
            if(!singetons.TryGetValue(typeof(T), out var output))
            {
                singetons.Add(typeof(T), output = typeof(T).GetConstructor(Type.EmptyTypes).Invoke(new object[] { }));
            }
            return (T)Convert.ChangeType(output, typeof(T));
        }
    }
}
