namespace Modules.Database.Generics
{
    public interface IDictCollection<T>
    {
        T Get(string key);
        T [] GetAll();
        T GetIfContains(string key, T returnOnDefault = default(T));
        bool Contains(string key);
        void Set(string key, T model);
        void Remove(string key);
        void ClearAll();
        void Save();
    }

    public interface IListCollection<T>
    {
        T Get(int i);
        T [] GetAll();
        void Append(T model);
        void Prepend(T model);
        void Insert(int i, T model);
        void RemoveAt(int i);
        void Remove(T model);
        void ClearAll();
        void Save();
    }

    public interface ISingletonCollection<T>
    {
        T Get();
        T GetIfContains(T returnOnDefault = default(T));
        void Set(T model);
        void Remove();
        void Save();
    }

    public interface IJsonCollection
    {
        E GetValue<E>(string key, E returnOnDefault = default(E));
        void SetValue<E>(string key, E value);
        void Remove(string key);
        void Save();
    }

    public class Storage
    {
        public class FILE : Storage { }
        public class PLAYER_PREFS : Storage { }
        public class SQLLITE : Storage { }
    }
}