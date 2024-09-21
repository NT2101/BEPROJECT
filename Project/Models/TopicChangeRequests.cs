using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    [Table("tblTopicChangeRequests")]
    public class TopicChangeRequest
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Student")]
        public string StudentID { get; set; }
        public Student Student { get; set; }
        [ForeignKey("Teacher")]
        public int TeacherID { get; set; }
        public Teacher teacher { get; set; }

        [ForeignKey("Topic")]
        public int TopicID { get; set; }
        public Topic Topic { get; set; }
        public int FieldID { get; set; }

        [ForeignKey("FieldID")]
        public Field Field { get; set; }

        [MaxLength(400)]
        public string NewTitle { get; set; }

        public string NewDescription { get; set; }

        [MaxLength(400)]
        public string ReasonForChange { get; set; }

        public DateTime RequestDate { get; set; }

        public int Status { get; set; } // 0: Pending, 1: Approved, 2: Rejected

        public DateTime? DecisionDate { get; set; }

        [MaxLength(50)]
        public string DecisionBy { get; set; } // Teacher who approved/rejected the request

        [MaxLength(400)]
        public string RejectionReason { get; set; }
    }
}
