namespace Modules.Database.Generics
{
    public interface IDBSerializer<T>
    {
        string Serialize(T t);
        T Deserialize(string r);
    }
}