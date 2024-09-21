
namespace Project.DTO
{
    public class CommitteeListDTO
    {
        public int CommitteeID { get; set; }
        public string CommitteeName { get; set; }
        public List<CommitteeTeacherDTO> Teachers { get; set; }
        public List<CommitteeStudentDTO> Students { get; set; }

       
    }
}
