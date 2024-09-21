using System.ComponentModel.DataAnnotations;

namespace Project.DTO.Request
{
    public class TopicRequest
    {
       public string Title { get; set; }
        public string Description { get; set; }
        public string StudentID { get; set; }
        public int TeacherID { get; set; }
        public int FieldID { get; set; }
    }
}
