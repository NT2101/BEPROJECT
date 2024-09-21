using System.ComponentModel.DataAnnotations;

namespace Project.DTO
{
    public class RegisterTopicTeacherDTO
    {
        [Required]
        public int TeacherID { get; set; }
        [Required]
        public int FieldID { get; set; }

        [Required]
        [MaxLength(400)]
        public string Title { get; set; }

        [MaxLength]
        public string Description { get; set; }
    }

}
