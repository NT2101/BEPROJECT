using Project.DTO;
using Project.Models;

namespace Project.Interfaces
{
    public interface ITeacherRepository
    {
        ICollection<Teacher> GetTeachers();
        TeachersDTO GetTeacher(int TeacherID);

        ICollection<Account> GetAccountByTeacher(int ID);
        bool TeacherExists(int id);
        bool CreateTeacher(Teacher Teacher);
        bool UpdateTeacher(Teacher Teacher);
        bool DeleteTeacher(Teacher Teacher);
        bool Save();

    }
}
