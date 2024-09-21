using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.Interfaces;
using Project.Models;

namespace Project.Repository
{
    public class StudentRepository : IStudentRepository
    {

        private DataContext _context;
        public StudentRepository(DataContext context)
        {
            _context = context;
        }
        public bool CreateStudent(Student Student)
        {
            _context.Add(Student);
            return Save();
        }

        public bool DeleteStudent(Student Student)
        {
            _context.Remove(Student);
            return Save();
        }

        public ICollection<Account> GetAccountByStudent(int ID)
        {
            return _context.Students.Where(e => e.AccountID == ID).Select(c => c.account).ToList();
        }

        public StudentDTO GetStudent(string studentID)
        {
            var student = _context.Students
                .Where(s => s.StudentID == studentID)
                .Select(s => new StudentDTO
                {
                    StudentID = s.StudentID,
                    Name = s.Name ?? "Chưa có",
                    Dob = s.Dob,
                    Sex = s.Sex,
                    Address = s.Address ?? "Chưa có",
                    PhoneNumber = s.PhoneNumber ?? "Chưa có",
                    EmailAddress = s.EmailAddress ?? "Chưa có",
                    Country = s.Country ?? "Chưa có",
                    ClassID = s.ClassID,
                })
                .FirstOrDefault();

            return student; // Return the DTO
        }



        public ICollection<Student> GetStudents()
        {
            return _context.Students.ToList();
        }

        public async Task<bool> IsStudentRegisteredForTopic(string studentID)
        {
            return await _context.Topics.AnyAsync(t => t.StudentID == studentID);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool StudentExists(string StudentID)
        {
            return _context.Students.Any(c => c.StudentID == StudentID);
        }

        public bool UpdateStudent(Student Student)
        {
            _context.Update(Student);
            return Save();
        }

       
    }
}
