using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.Models;

[Route("api/[controller]")]
[ApiController]
public class TopicChangeRequestsController : ControllerBase
{
    DataContext _context;

    public TopicChangeRequestsController(DataContext context)
    {
        _context = context;
    }

    // POST: api/TopicChangeRequests
    [HttpPost]
    public async Task<ActionResult<TopicChangeRequest>> RequestTopicChange(TopicChangeRequestDTO requestDto)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentID == requestDto.StudentID);

        if (student == null)
        {
            return BadRequest("Student not found.");
        }

        var currentTopic = await _context.Topics
            .FirstOrDefaultAsync(t => t.StudentID == requestDto.StudentID);

        if (currentTopic == null)
        {
            return BadRequest("No assigned topic found for the student.");
        }

        var topicChangeRequest = new TopicChangeRequest
        {
            StudentID = requestDto.StudentID,
            TopicID = currentTopic.ID,
            TeacherID = currentTopic.TeacherID,
            FieldID = requestDto.FieldID,
            NewTitle = requestDto.NewTitle,
            NewDescription = requestDto.NewDescription,
            ReasonForChange = requestDto.ReasonForChange,
            RequestDate = DateTime.UtcNow,
            Status = 0, // Pending
            DecisionDate = DateTime.UtcNow,
            DecisionBy = currentTopic.Student.Name,
            RejectionReason = "API" // Fixed syntax error
        };

        _context.TopicChangeRequests.Add(topicChangeRequest);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the exception details
            Console.WriteLine(ex.ToString());
            return StatusCode(500, "An error occurred while processing the request.");
        }

        return CreatedAtAction(nameof(GetTopicChangeRequest), new { id = topicChangeRequest.ID }, topicChangeRequest);
    }



    // GET: api/TopicChangeRequests/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TopicChangeRequest>> GetTopicChangeRequest(int id)
    {
        var topicChangeRequest = await _context.TopicChangeRequests.FindAsync(id);

        if (topicChangeRequest == null)
        {
            return NotFound();
        }

        return Ok(topicChangeRequest);
    }
    [HttpGet("teacher/{teacherID}")]
    public async Task<IActionResult> GetRequestsByTeacher(int teacherID)
    {
        try
        {
            // Log the teacherID to ensure it is being received correctly
            Console.WriteLine($"Received teacherID: {teacherID}");

            var requests = await _context.TopicChangeRequests
                .Where(r => r.TeacherID == teacherID)
                .Select(r => new
                {
                    r.ID,
                    r.NewTitle,
                    r.NewDescription,
                    r.ReasonForChange,
                    r.Field.FieldName,
                    r.Status,
                    Student = new
                    {
                        r.Student.StudentID,
                        r.Student.Name
                    }
                })
                .ToListAsync();

            return Ok(requests);
        }
        catch (Exception ex)
        {
            // Log the exception (not shown here)
            return StatusCode(500, "Lỗi khi lấy yêu cầu thay đổi đề tài.");
        }
    }



    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<int?>> GetStatusByStudentId(string studentId)
    {
        try
        {
            // Lấy Status của yêu cầu thay đổi đề tài của sinh viên
            var status = await _context.TopicChangeRequests
                .Where(t => t.StudentID == studentId)
                 .Select(r => new
                 {
                     r.ID, // Assuming you also want to return ID
                     r.NewTitle,
                     r.Field.FieldName,
                     r.NewDescription,
                     r.ReasonForChange,
                     r.Status,
                     Student = new
                     {
                         r.Student.StudentID,
                         r.Student.Name // Assuming the student's name is stored in the `Name` property
                     }
                 })
                .FirstOrDefaultAsync();

            // Nếu không có yêu cầu thay đổi đề tài
            if (status == null)
            {
                return NotFound("No topic change request found for the given student ID.");
            }

            return Ok(status);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.ToString()}");
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }


    

    [HttpPut("Agree/{topicID}")]
    public async Task<IActionResult> Agree(int topicID)
    {
        try
        {
            // Find the topic
            var request = await _context.TopicChangeRequests.FindAsync(topicID);
            if (request == null)
            {
                return NotFound("Topic not found");
            }
            var topic = await _context.Topics.FindAsync(request.TopicID);
            if (topic == null)
            {
                return NotFound("Topic not found");
            }

            // Update the topic status to 5
            topic.status = 5;

            // Update the status of the topic
            request.Status = 1; // Assuming 2 means "Agreed"

            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    [HttpPut("Disagree/{topicID}")]
    public async Task<IActionResult> Disagree(int topicID)
    {
        try
        {
            // Find the topic
            var request = await _context.TopicChangeRequests.FindAsync(topicID); 
            if (request == null)
            {
                return NotFound("Topic not found");
            }

            // Update the status of the topic
            request.Status = 2; // Assuming 3 means "Disagreed"

            // Find the student associated with the topic
         

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }



}
