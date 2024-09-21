using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    [Table("tblCommittee")]
    public class Committee
    {
        [Key]
        public int CommitteeID { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public int? StudentCount { get; set; }
        public ICollection<CommitteeTeacherMember> CommitteeTeacherMembers { get; set; }
        public ICollection<CommitteeStudentMember> CommitteeStudentMembers { get; set; }

    }

}

