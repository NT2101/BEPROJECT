using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    [Table("tblFields")]//Quản lý lĩnh vực

    public class Field
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(100)]
        public string FieldName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(50)]
        public string CreatedUser { get; set; }
        public DateTime ModifiedDate { get; set; }
        [MaxLength(50)]
        public string ModifiedUser { get; set; }
    }
}
