using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.DTO.Request;
using Project.DTOs;
using Project.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressReportController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly string _fileStoragePath = @"E:\Process";

        public ProgressReportController(DataContext context)
        {
            _context = context;
        }
        [HttpPost("SubmitReport")]
        public async Task<IActionResult> SubmitReport([FromForm] ProgressReportFileDTO reportDto)
        {
            var currentDateTime = DateTime.Now;

            var currentProgress = await _context.Progresses
                .Where(p => p.StartDate <= currentDateTime && p.EndDate >= currentDateTime)
                .FirstOrDefaultAsync();

            if (currentProgress == null)
            {
                return BadRequest("No active progress period. You cannot submit the report.");
            }

            if (reportDto.File == null || reportDto.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Get TeacherID based on StudentID from the Topic table
            var topic = await _context.Topics
                .Where(t => t.StudentID == reportDto.StudentID)
                .FirstOrDefaultAsync();

            if (topic == null)
            {
                return BadRequest("No topic found for the provided StudentID.");
            }

            // Tạo đường dẫn tệp mà không sử dụng Guid
            var filePath = Path.Combine(_fileStoragePath, reportDto.File.FileName);
            Directory.CreateDirectory(_fileStoragePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await reportDto.File.CopyToAsync(stream);
            }

            // Create and save the progress report
            var progressReport = new ProgressReport
            {
                StudentID = reportDto.StudentID,
                TeacherID = topic.TeacherID, // Use TeacherID from the Topic
                SubmissionDate = currentDateTime,
                FilePath = filePath,
                CreatedDate = currentDateTime,
                CreatedUser = "API",
                ModifiedDate = currentDateTime,
                ModifiedUser = "API",
                ProgressID = currentProgress.ProgressID
            };

            _context.ProgressReports.Add(progressReport);

            // Update StatusProgess for the student
            var student = await _context.Students
                .Where(s => s.StudentID == reportDto.StudentID)
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return BadRequest("No student found for the provided StudentID.");
            }

            student.StatusProgess = 2;

            _context.Students.Update(student);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(progressReport);
        }
        [HttpGet("GetSubmittedFiles/{studentID}")]
        public async Task<IActionResult> GetSubmittedFiles(string studentID)
        {
            // Tìm kiếm các báo cáo tiến độ dựa trên StudentID
            var progressReports = await _context.ProgressReports
                .Where(pr => pr.StudentID == studentID)
                .Select(pr => new
                {pr.ProgressID,
                pr.Progress.Title,
                    pr.Comments,
                    pr.ReportID,
                    pr.FilePath,
                    FileName = Path.GetFileName(pr.FilePath) // Lấy tên tệp từ đường dẫn tệp
                })
                .ToListAsync();

            if (progressReports == null || !progressReports.Any())
            {
                return NotFound("No submitted files found for the provided StudentID.");
            }

            return Ok(progressReports);
        }



        [HttpPut("AddComment/{reportId}")]
        public async Task<IActionResult> AddComment(int reportId, [FromBody] CommentDTO commentDTO)
        {
            var report = await _context.ProgressReports
                .FindAsync(reportId);

            if (report == null)
            {
                return NotFound("Report not found.");
            }

            // Nếu commentDTO.Comment có thể là NULL, hãy kiểm tra và xử lý nó
            report.Comments = commentDTO.Comment ?? string.Empty; // Đặt giá trị mặc định nếu commentDTO.Comment là NULL
            report.ModifiedDate = DateTime.Now; // Cập nhật thời gian chỉnh sửa

            _context.ProgressReports.Update(report);
            await _context.SaveChangesAsync();

            return Ok("Comment added successfully.");
        }






        [HttpGet("GetReportsByProgress/{progressId}")]
        public async Task<IActionResult> GetReportsByProgress(int progressId)
        {
            var reports = await _context.ProgressReports
                .Include(t => t.Student)
                .Include(t => t.Teacher)
                .Include(t => t.Progress)
                .Where(t => t.ProgressID == progressId)
                .Select(t => new GetProgessByProgessIDDTO
                {
                    StudentID = t.StudentID,
                    StudentName = t.Student.Name,
                    TeacherID = t.TeacherID,
                    TeacherName = t.Teacher.Name,
                    ProgressID = t.ProgressID,
                    ProgessName = t.Progress.Title,
                    SubmissionDate = t.SubmissionDate,
                    ReportID = t.ReportID,  // Bao gồm ReportID
                    FileName = Path.GetFileName(t.FilePath)  // Lấy tên tệp từ đường dẫn tệp
                })
                .ToListAsync();  // Sử dụng ToListAsync() cho phương thức async

            if (reports == null || !reports.Any())
            {
                return NotFound();
            }

            return Ok(reports);
        }


        [HttpGet("DownloadReport/{reportId}")]
        public async Task<IActionResult> DownloadReport(int reportId)
        {
            if (reportId <= 0)
            {
                return BadRequest("Invalid report ID.");
            }

            try
            {
                // Kiểm tra xem _context có phải là null không
                if (_context == null)
                {
                    Console.WriteLine("Database context is not initialized.");
                    return StatusCode(500, "Database context is not initialized.");
                }

                // Tìm báo cáo bằng reportId và chỉ lấy ra FilePath để tránh lỗi khi các trường khác là null
                var report = await _context.ProgressReports
                    .Where(r => r.ReportID == reportId)
                    .Select(r => new { r.FilePath })
                    .FirstOrDefaultAsync();

                // Kiểm tra nếu báo cáo không tồn tại
                if (report == null)
                {
                    Console.WriteLine($"Report with ID {reportId} not found.");
                    return NotFound("Report not found.");
                }

                // Kiểm tra nếu đường dẫn tệp rỗng hoặc null
                if (string.IsNullOrEmpty(report.FilePath))
                {
                    Console.WriteLine($"Report with ID {reportId} has a null or empty file path.");
                    return BadRequest("File path is null or empty.");
                }

                // Tạo đường dẫn đầy đủ đến tệp
                var filePath = Path.Combine(_fileStoragePath, Path.GetFileName(report.FilePath));

                // Kiểm tra nếu tệp không tồn tại
                if (!System.IO.File.Exists(filePath))
                {
                    Console.WriteLine($"File at path {filePath} does not exist.");
                    return NotFound("File does not exist.");
                }

                // Đọc nội dung tệp
                byte[] fileBytes;
                try
                {
                    fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                }
                catch (Exception ex)
                {
                    // Ghi lỗi ra log và trả về thông báo lỗi
                    Console.WriteLine($"Error reading file: {ex.Message}");
                    return StatusCode(500, $"Error reading file: {ex.Message}");
                }

                // Trả về tệp với đúng MIME type và tên tệp
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                // Ghi lỗi ra log và trả về thông báo lỗi
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, $"Error accessing database: {ex.Message}");
            }
        }


        [HttpGet("GetReportsByProgressAndTeacher/{progressID}/{teacherID}")]
        public async Task<IActionResult>  GetReportsByProgressAndTeacher(int progressID, int teacherID)
        {
            var reports = await _context.ProgressReports
                .Include(r => r.Student)
                .Include(r => r.Teacher)
                .Include(r => r.Progress)
                .Where(r => r.ProgressID == progressID && r.TeacherID == teacherID)
                .Select(r => new GetProgressByTeacherIDDTO
                {
                    ReportID = r.ReportID,
                    StudentID = r.StudentID,
                    StudentName = r.Student.Name,
                    TeacherID = r.TeacherID,
                    TeacherName = r.Teacher.Name,
                    ProgressID = r.ProgressID,
                    ProgressTitle = r.Progress.Title,
                    SubmissionDate = r.SubmissionDate,
                    FilePath = r.FilePath,
                   FileName = Path.GetFileName(r.FilePath),
                   Comments= r.Comments
                })
                .ToListAsync();

            if (reports == null || reports.Count == 0)
            {
                return NotFound("No reports found for the selected progress and teacher.");
            }

            return Ok(reports);
        }
    

    


        [HttpGet("GetAllProgresses")]
        public async Task<IActionResult> GetAllProgresses()
        {
            var progresses = await _context.Progresses.ToListAsync();
            return Ok(progresses);
        }
    }
}
