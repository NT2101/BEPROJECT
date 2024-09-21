using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs;
using Project.Models;

[Route("api/[controller]")]
[ApiController]
public class TeacherReviewController : ControllerBase
{
    private readonly DataContext _context;

    public TeacherReviewController(DataContext context)
    {
        _context = context;
    }

    // GET: api/TeacherReview/GetReportsByTeacher/5
    [HttpGet("GetProgressesWithReports/{teacherID}")]
    public async Task<ActionResult<IEnumerable<ProgressDTO>>> GetProgressesWithReports(int teacherID)
    {
        var progressesWithReports = await _context.ProgressReports
            .Where(pr => pr.TeacherID == teacherID)
            .Select(pr => pr.Progress)
            .Distinct()
            .Select(p => new ProgressDTO
            {
                ProgressID = p.ProgressID,
                Title = p.Title
            })
            .ToListAsync();

        if (progressesWithReports == null || progressesWithReports.Count == 0)
        {
            return NotFound("No progresses found with reports for the specified teacher.");
        }

        return Ok(progressesWithReports);
    }

    // GET: api/TeacherReview/GetReport/5
    [HttpGet("GetReport/{id}")]
    public async Task<ActionResult<ProgressReport>> GetReport(int id)
    {
        var progressReport = await _context.ProgressReports
            .Include(r => r.Student)
            .FirstOrDefaultAsync(r => r.ReportID == id);

        if (progressReport == null)
        {
            return NotFound();
        }

        return progressReport;
    }

    // PUT: api/TeacherReview/AddComment/5
    [HttpPut("AddComment/{id}")]
    public async Task<IActionResult> AddComment(int id, [FromBody] string comment)
    {
        try
        {
            var progressReport = await _context.ProgressReports.FindAsync(id);

            if (progressReport == null)
            {
                return NotFound("Progress report not found.");
            }

            // Handle nullable comments
            progressReport.Comments = comment ?? "No comment provided";
            progressReport.ModifiedDate = DateTime.Now; // Ensure ModifiedDate is handled correctly
            progressReport.ModifiedUser = User.Identity?.Name ?? "Unknown"; // Safeguard against null User.Identity

            _context.Entry(progressReport).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent(); // Successful update without content
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    // GET: api/TeacherReview/DownloadFile/5
    [HttpGet("DownloadFile/{id}")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var progressReport = await _context.ProgressReports.FindAsync(id);

        if (progressReport == null || string.IsNullOrEmpty(progressReport.FilePath))
        {
            return NotFound();
        }

        var filePath = progressReport.FilePath;
        var fileName = Path.GetFileName(filePath);
        var mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; // Assuming it's a Word file

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

        return File(fileBytes, mimeType, fileName);
    }
}
