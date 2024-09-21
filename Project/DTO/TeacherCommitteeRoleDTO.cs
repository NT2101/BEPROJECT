namespace Project.Models
{
    public class TeacherCommitteeRoleDTO
    {
        public int TeacherId { get; set; }
        public int Role { get; set; } // 0: Chair, 1: Secretary, 2: Member
    }
}
