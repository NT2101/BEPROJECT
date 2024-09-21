using Project.Models;

namespace Project.Interfaces
{
    public interface IClassRepository
    {
        ICollection<Class> GetClasses();
        Class GetClass(String id);
        bool ClassExists(String id);
        bool CreateClass(Class classEntity);
        bool UpdateClass(Class classEntity);
        bool DeleteClass(Class classEntity);
        bool HasAssociatedClass(string classID);
        bool Save();
    }
}
