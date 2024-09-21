using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    [Table("tblProgressReport")]
    public class ProgressReport
    {
        [Key]
        public int ReportID { get; set; }

        [ForeignKey("Student")]
        public string StudentID { get; set; }
        public Student Student { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherID { get; set; }
        public Teacher Teacher { get; set; }

        public DateTime SubmissionDate { get; set; }
        public string FilePath { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Comments { get; set; }

        public DateTime CreatedDate { get; set; }

        [MaxLength(50)]
        public string CreatedUser { get; set; }

        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string ModifiedUser { get; set; }

        [ForeignKey("Progress")]
        public int ProgressID { get; set; }
        public Progress Progress { get; set; }
    }
}
