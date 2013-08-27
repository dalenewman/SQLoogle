namespace Sqloogle
{
    public interface IIndexWriter {
        void Clean();
        void Add(object doc);
        void Delete(string id);
        void Update(string id, object doc);
    }
}