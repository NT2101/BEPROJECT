using System.ComponentModel.DataAnnotations;

namespace Project.DTO
{
    public class TopicDTO
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string StudentID { get; set; }
        public int Status { get; set; }

        public string TeacherID { get; set; }
        public string TeacherName { get; set; }

        public string Description { get; set; }
        public string StudentName { get; set; }
        public string StudentClass { get; set; }
    }
}
