using Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
[Table("tblCommitteeTeacherMembers")]
public class CommitteeTeacherMember
{
    public int CommitteeTeacherMemberID { get; set; }
    public int CommitteeID { get; set; }
    public int TeacherID { get; set; }
    public int Role { get; set; } // 0: Chair, 1: Secretary, 2: Member

    public Committee Committee { get; set; }
    public Teacher Teacher { get; set; }
}
