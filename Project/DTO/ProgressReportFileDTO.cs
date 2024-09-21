using Microsoft.AspNetCore.Http;

namespace Project.DTOs
{
    public class ProgressReportFileDTO
    {
        public string StudentID { get; set; }
        public IFormFile File { get; set; }
    }
}
