using Project.Models;

namespace Project.Interfaces
{
    public interface IFieldRepository
    {
        ICollection<Field> GetFields();
        Field GetField(int id);
        bool FieldExists(int id);
        bool CreateField(Field field);
        bool UpdateField(Field field);
        bool DeleteField(Field field);
        bool Save();
    }
}
