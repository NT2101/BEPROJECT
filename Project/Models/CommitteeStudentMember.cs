using Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
[Table("tblCommitteeStudentMembers ")]

public class CommitteeStudentMember
{
    public int CommitteeStudentMemberID { get; set; }
    public int CommitteeID { get; set; }
    public string StudentID { get; set; }

    public Committee Committee { get; set; }
    public Student Student { get; set; }
}
