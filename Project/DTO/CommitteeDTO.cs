namespace Project.DTO
{
    public class CommitteeDTO
    {
        public int CommitteeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? StudentCount { get; set; }
        public List<CommitteeTeacherssDTO> Teachers { get; set; }
        public List<CommitteeStudentssDTO> Students { get; set; }
    }
}
