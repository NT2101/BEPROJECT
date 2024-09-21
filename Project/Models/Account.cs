    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    using System.Data;

    namespace Project.Models
    {
        [Table("tblAccount")]//Quản lý tài khoản

        public class Account
        {
            [Key]
            public int ID { get; set; }
            [MaxLength(50)]
            public string? Name { get; set; }
            [MaxLength(50)]
            public string? Password { get; set; }
            [ForeignKey("tblRole")]
            public int RoleID { get; set; }
            public int Status { get; set; }
            public DateTime? CreatedDate { get; set; }
            [MaxLength(50)]
            public string CreatedUser { get; set; }
            public DateTime? ModifiedDate { get; set; }
            [MaxLength(50)]
            public string ModifiedUser { get; set; }

            public Role Role { get; set; }
        }
    }
