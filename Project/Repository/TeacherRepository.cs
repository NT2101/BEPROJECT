using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.Interfaces;
using Project.Models;


namespace Project.Repository
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TeacherRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public bool CreateTeacher(Teacher Teacher)
        {
            _context.Add(Teacher);
            return Save();
        }

        public bool DeleteTeacher(Teacher Teacher)
        {
            _context.Remove(Teacher);
            return Save();
        }

        public ICollection<Account> GetAccountByTeacher(int ID)
        {
            return _context.Teachers.Where(e => e.AccountID == ID).Select(c => c.account).ToList();
        }

        public TeachersDTO GetTeacher(int TeacherID)
        {
            var student = _context.Teachers
                .Where(s => s.TeacherID == TeacherID)
                .Select(s => new TeachersDTO
                {
                    TeacherID = s.TeacherID,
                    Name = s.Name ?? "Chưa có",
                    Dob = s.Dob,
                    Sex = s.Sex,
                    Description = s.Description ?? "Chưa có",
                    PhoneNumber = s.PhoneNumber ?? "Chưa có",
                    EmailAddress = s.EmailAddress ?? "Chưa có",
                    FacultyID = s.FacultyID,
                    Status= s.Status
                })
                .FirstOrDefault();

            return student; // Return the DTO
        }

        public ICollection<Teacher> GetTeachers()
        {
            return _context.Teachers.ToList() ?? new List<Teacher>();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool TeacherExists(int id)
        {
            return _context.Teachers.Any(c => c.TeacherID == id);
        }

        public bool UpdateTeacher(Teacher Teacher)
        {
            _context.Update(Teacher);
            return Save();
        }
    }
}
