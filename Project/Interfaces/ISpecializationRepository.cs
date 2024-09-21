using Project.Models;

namespace Project.Interfaces
{
    public interface ISpecializationRepository
    {
        ICollection<Specialization> GetSpecializations();
        Specialization GetSpecialization(String id);
        bool CreateSpecialization(Specialization specialization);
        bool UpdateSpecialization(Specialization specialization);
        bool DeleteSpecialization(Specialization specialization);
        bool HasAssociatedClass(string specializationId);

        bool Save();
    }
}
