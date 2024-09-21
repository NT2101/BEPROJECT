using System.Collections.Generic;
using System.Linq;
using Project.Data;
using Project.Interfaces;
using Project.Models;

namespace Project.Repositories
{
    public class FieldRepository : IFieldRepository
    {
        private readonly DataContext _context;

        public FieldRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Field> GetFields()
        {
            return _context.Fields.ToList();
        }

        public Field GetField(int id)
        {
            return _context.Fields.Find(id);
        }

        public bool FieldExists(int id)
        {
            return _context.Fields.Any(f => f.ID == id);
        }

        public bool CreateField(Field field)
        {
            _context.Fields.Add(field);
            return Save();
        }

        public bool UpdateField(Field field)
        {
            _context.Fields.Update(field);
            return Save();
        }

        public bool DeleteField(Field field)
        {
            _context.Fields.Remove(field);
            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
