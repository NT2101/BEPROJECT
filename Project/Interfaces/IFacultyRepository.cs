using Project.Models;

namespace Project.Interfaces
{
    public interface IFacultyRepository
    {
        ICollection<Faculty> GetFaculties();
        Faculty GetFaculty(String id);
        bool FacultyExists(String id);
        bool CreateFaculty(Faculty faculty);
        bool UpdateFaculty(Faculty faculty);
        bool DeleteFaculty(Faculty faculty);
        bool HasAssociatedSpecializations(string facultyId);
        bool Save();
    }
}
