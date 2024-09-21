using System.ComponentModel.DataAnnotations;

namespace Project.DTO.Request
{
    public class SpecializationRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public String FacultyID { get; set; }
    }
}
