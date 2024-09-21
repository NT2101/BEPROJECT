using System.Collections.Generic;
using System.Linq;
using Project.Data;
using Project.Interfaces;
using Project.Models;

namespace Project.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly DataContext _context;

        public ClassRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Class> GetClasses()
        {
            return _context.Classes.ToList();
        }

        public Class GetClass(String id)
        {
            return _context.Classes.Find(id);
        }

        public bool ClassExists(String id)
        {
            return _context.Classes.Any(c => c.ID == id);
        }

        public bool CreateClass(Class classEntity)
        {
            _context.Classes.Add(classEntity);
            return Save();
        }

        public bool UpdateClass(Class classEntity)
        {
            _context.Classes.Update(classEntity);
            return Save();
        }

        public bool DeleteClass(Class classEntity)
        {
            _context.Classes.Remove(classEntity);
            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public bool HasAssociatedClass(string classID)
        {
            return _context.Students.Any(s => s.ClassID == classID);
        }
    }
}
