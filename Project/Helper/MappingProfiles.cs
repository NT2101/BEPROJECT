using AutoMapper;
using Project.DTO;
using Project.DTO.Request;
using Project.Models;

namespace Project.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<Class, ClassDTO>().ReverseMap();
            CreateMap<Faculty, FacultyDTO>().ReverseMap();
            CreateMap<Field, FieldDTO>().ReverseMap();
            CreateMap<Specialization, SpecializationDTO>().ReverseMap();
            CreateMap<Student, StudentDTO>().ReverseMap();
            CreateMap<Teacher, TeachersDTO>().ReverseMap();
            CreateMap<Topic, TopicDTO>().ReverseMap();
            CreateMap<FacultyRequest, Faculty>().ReverseMap();
            CreateMap<SpecializationRequest, Specialization>().ReverseMap();
        }
    }
}
