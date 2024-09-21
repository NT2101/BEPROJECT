using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    [Table("tblSpecializations")]//Danh mục chuyên ngành

    public class Specialization
    {
        [Key]
        public string ID { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }

        public string FacultyID { get; set; } 

        [ForeignKey("FacultyID")]
        public Faculty Faculty { get; set; } 
        public ICollection<Class> Classes { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(50)]
        public string CreatedUser { get; set; }
        public DateTime ModifiedDate { get; set; }
        [MaxLength(50)]
        public string ModifiedUser { get; set; }
    }

}
