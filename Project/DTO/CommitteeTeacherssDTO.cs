namespace Project.DTO
{
    public class CommitteeTeacherssDTO
    {
        public int TeacherID { get; set; }
        public string TeacherName { get; set; }
        public int Role { get; set; } // 0: Chair, 1: Secretary, 2: Member
    }
}
