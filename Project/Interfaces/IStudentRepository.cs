using Project.DTO;
using Project.Models;

namespace Project.Interfaces
{
    public interface IStudentRepository
    {
        Task<bool> IsStudentRegisteredForTopic(string studentID);
        ICollection<Student> GetStudents();
        StudentDTO GetStudent(string StudentID);
        ICollection<Account> GetAccountByStudent(int ID);
        bool StudentExists(string StudentID);
        bool CreateStudent(Student Student);
        bool UpdateStudent(Student Student);
        bool DeleteStudent(Student Student);
        bool Save();
    }
}
