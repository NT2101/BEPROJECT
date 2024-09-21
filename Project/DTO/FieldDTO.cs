using System.ComponentModel.DataAnnotations;

namespace Project.DTO
{
    public class FieldDTO
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "FieldName is required")]
        [MaxLength(100)]
        public string FieldName { get; set; }
        public string Description { get; set; }
 
    }
}
