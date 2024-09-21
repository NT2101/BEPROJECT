using System;
using System.Collections.Generic;
using System.Linq;
using Project.Data;
using Project.Interfaces;
using Project.Models;

namespace Project.Repositories
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly DataContext _context;

        public SpecializationRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Specialization> GetSpecializations()
        {
            return _context.Specializations.ToList();
        }

        public Specialization GetSpecialization(String id)
        {
            return _context.Specializations.Find(id);
        }

        public bool CreateSpecialization(Specialization specialization)
        {
            try
            {
                _context.Specializations.Add(specialization);
                return Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateSpecialization: {ex.Message}");
                return false;
            }
        }

        public bool UpdateSpecialization(Specialization specialization)
        {
            try
            {
                _context.Specializations.Update(specialization);
                return Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateSpecialization: {ex.Message}");
                return false;
            }
        }

        public bool DeleteSpecialization(Specialization specialization)
        {
            try
            {
                _context.Specializations.Remove(specialization);
                return Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteSpecialization: {ex.Message}");
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Save changes: {ex.Message}");
                return false;
            }
        }

        public bool HasAssociatedClass(string specializationId)
        {
            return _context.Classes.Any(s => s.SpecializationID == specializationId);
        }
    }
}
