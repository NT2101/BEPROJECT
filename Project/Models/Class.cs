using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    [Table("tblClasses")]//Quản lý lớp học

    public class Class
    {
        [Key]
        public string ID { get; set; }
        [MaxLength(100)]
        public string ClassName { get; set; }

        public string Description { get; set; }

        public string SpecializationID { get; set; }


        [ForeignKey("SpecializationID")]
        public Specialization Specialization { get; set; } 
        public ICollection<Student> Students { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(50)]
        public string CreatedUser { get; set; }
        public DateTime ModifiedDate { get; set; }
        [MaxLength(50)]
        public string ModifiedUser { get; set; }
    }
}
