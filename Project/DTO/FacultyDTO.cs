using System.ComponentModel.DataAnnotations;

namespace Project.DTO
{
    public class FacultyDTO
    {
        public String ID { get; set; }
        [Required(ErrorMessage = "FacultyName is required")]
        [MaxLength(100)]
        public string FacultyName { get; set; }
        public string Description { get; set; }

    }
}
