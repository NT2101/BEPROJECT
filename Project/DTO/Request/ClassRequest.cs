using System.ComponentModel.DataAnnotations;

namespace Project.DTO.Request
{
    public class ClassRequest
    {
        public string ClassName { get; set; }
        public string SpecializationID { get; set; }

        public string Description { get; set; }
    }
}
