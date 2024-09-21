using System.Collections.Generic;
using System.Linq;
using Project.Data;
using Project.Interfaces;
using Project.Models;

namespace Project.Repositories
{

    public class FacultyRepository : IFacultyRepository
    {
        private readonly DataContext _context;

        public FacultyRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Faculty> GetFaculties()
        {
            return _context.Faculties.ToList();
        }

        public Faculty GetFaculty(String id)
        {
            return _context.Faculties.Find(id);
        }

        public bool FacultyExists(String id)
        {
            return _context.Faculties.Any(f => f.ID == id);
        }

        public bool CreateFaculty(Faculty faculty)
        {
            if (string.IsNullOrEmpty(faculty.CreatedUser))
            {
                faculty.CreatedUser = "default_user"; // hoặc raise error
            }

            _context.Faculties.Add(faculty);
            return Save();
        }


        public bool UpdateFaculty(Faculty faculty)
        {
            _context.Faculties.Update(faculty);
            return Save();
        }

        public bool DeleteFaculty(Faculty faculty)
        {
            _context.Faculties.Remove(faculty);
            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public bool HasAssociatedSpecializations(string facultyId)
        {
            return _context.Specializations.Any(s => s.FacultyID == facultyId);
        }
    }
}
