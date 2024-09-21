using AutoMapper;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Project.Data;
using Project.DTO;
using Project.DTO.Request;
using Project.Interfaces;
using Project.Models;
using System.Linq;


namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : Controller
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IMapper _mapper;
        private readonly DataContext _context;


        public TopicController(ITopicRepository topicRepository, IMapper mapper, DataContext context)
        {
            _topicRepository = topicRepository;
            _mapper = mapper;
            _context = context;
        }
        // GET: api/Topics/{studentId}
        // GET: api/Topics/student/{studentId}
        [HttpGet("checkstudent/{studentID}")]
        public IActionResult GetTopicByStudent(string studentID)
        {
            // Check if the student has an assigned topic
            var topic = _context.Topics.FirstOrDefault(t => t.StudentID == studentID);

            if (topic == null)
            {
                // No topic assigned to this student
                return NotFound(new { message = "Student does not have an assigned topic." });
            }

            // Return the topic details
            return Ok(topic);
        }
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<Topic>> GetTopicByStudentId(string studentId)
        {
            var topic = await _context.Topics
                .FirstOrDefaultAsync(t => t.StudentID == studentId);

            if (topic == null)
            {
                return NotFound("No topic assigned for the given student ID.");
            }

            return Ok(topic);
        }

        [HttpGet("GetAllTopics")]
        public async Task<IActionResult> GetAllTopics()
        {
            try
            {
                // Truy vấn các đề tài từ bảng TopicChangeRequests với status = 1
                var topicsFromRequests = _context.TopicChangeRequests
                    .Join(_context.Topics,
                          r => r.TopicID,
                          t => t.ID,
                          (r, t) => new TopicGetAllDTO
                          {
                              ID = r.ID,
                              Title = r.NewTitle, // NewTitle từ TopicChangeRequests
                              Description = r.NewDescription, // NewDescription từ TopicChangeRequests
                              StudentID = t.StudentID,
                              StudentName = r.Student.Name, // Giả sử bạn có trường FullName trong Student
                              TeacherName = r.teacher.Name, // Giả sử bạn có trường FullName trong Teacher
                              ClassID = r.Student.ClassID, // Giả sử bạn có ClassID trong Student
                              Status = r.Status, // Status từ TopicChangeRequests
                          })
                    .Where(x => x.Status == 1 )
                    .AsQueryable();

                // Truy vấn các đề tài từ bảng Topics với status = 4
                var topicsFromTopics = _context.Topics
                    .Select(t => new TopicGetAllDTO
                    {
                        ID = t.ID,
                        Title = t.Title, // Title từ Topics
                        Description = t.Description, // Description từ Topics
                        StudentID = t.StudentID,
                        StudentName = t.Student.Name, // Giả sử bạn có trường FullName trong Student
                        TeacherName = t.Teacher.Name, // Giả sử bạn có trường FullName trong Teacher
                        ClassID = t.Student.ClassID, // Giả sử bạn có ClassID trong Student
                        Status = t.status, // Status từ Topics
                    })
                    .Where(t => t.Status == 4)
                    .AsQueryable();

                // Kết hợp các kết quả từ hai truy vấn
                var allTopics = topicsFromRequests.Concat(topicsFromTopics);

                // Chuyển đổi kết quả thành danh sách và trả về
                return Ok(await allTopics.ToListAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetTopicsByStatus")]
        public async Task<IActionResult> GetTopicsByStatus()
        {
            try
            {
                // Truy vấn để lấy các đề tài có status = 2 hoặc 4
                var topics = await _context.Topics
                    .Where(t => t.status == 2 || t.status == 4 || t.status ==1)
                    .Select(t => new
                    {
                        t.ID,
                        t.Title,
                        t.Description,
                        t.CreatedDate,
                        t.CreatedUser,
                        t.status,
                        t.StudentID,
                        t.TeacherID,
                        t.FieldID,
                        t.RegistrationDate,
                        StudentName = t.Student != null ? t.Student.Name : "N/A",  // Lấy tên sinh viên nếu có
                        TeacherName = t.Teacher != null ? t.Teacher.Name : "N/A",  // Lấy tên giáo viên nếu có
                        FieldName = t.Field != null ? t.Field.FieldName : "N/A"         // Lấy tên lĩnh vực nếu có
                    })
                    .ToListAsync();

                // Trả về danh sách các đề tài nếu tìm thấy
                return Ok(topics);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, trả về thông báo lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("ExportToExcel")]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                // Truy vấn dữ liệu từ cơ sở dữ liệu
                var topicsFromRequests = await (from r in _context.TopicChangeRequests
                                                join t in _context.Topics
                                                on r.TopicID equals t.ID
                                                where r.Status == 1
                                                select new
                                                {
                                                    ID = r.ID,
                                                    Title = r.NewTitle,
                                                    Description = r.NewDescription,
                                                    StudentID = t.StudentID,
                                                    StudentName = t.Student.Name,
                                                    TeacherName = t.Teacher.Name,
                                                    ClassID = t.Student.ClassID,
                                                }).ToListAsync();

                var topicsFromTopics = await (from t in _context.Topics
                                              where t.status == 4
                                              select new
                                              {
                                                  ID = t.ID,
                                                  Title = t.Title,
                                                  Description = t.Description,
                                                  StudentID = t.StudentID,
                                                  StudentName = t.Student.Name,
                                                  TeacherName = t.Teacher.Name,
                                                  ClassID = t.Student.ClassID,
                                              }).ToListAsync();

                // Kết hợp các kết quả từ hai truy vấn
                var allTopics = topicsFromRequests.Concat(topicsFromTopics).ToList();

                // Tạo file Excel
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Topics");

                    // Đặt tiêu đề trên cùng (A1-A3)
                    worksheet.Cells[1, 1, 3, 7].Merge = true;
                    worksheet.Cells[1, 1].Value = "Danh sách hướng dẫn";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.Font.Size = 16;
                    worksheet.Cells[1, 1].Style.Font.Name = "Times New Roman";
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // Đặt tiêu đề cột bắt đầu từ dòng 4
                    var headers = new string[] { "STT", "Mã sinh viên", "Tên sinh viên", "Giảng viên", "Tên đề tài", "Mô tả", "Lớp" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[4, i + 1].Value = headers[i];
                        worksheet.Cells[4, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[4, i + 1].Style.Font.Size = 14;
                        worksheet.Cells[4, i + 1].Style.Font.Name = "Times New Roman";
                        worksheet.Cells[4, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[4, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        // Thêm viền cho tiêu đề cột
                        worksheet.Cells[4, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // Căn giữa cột STT
                    worksheet.Cells[4, 1, allTopics.Count() + 4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Đặt dữ liệu vào các ô
                    for (int i = 0; i < allTopics.Count(); i++)
                    {
                        worksheet.Cells[i + 5, 1].Value = i + 1;
                        worksheet.Cells[i + 5, 2].Value = allTopics.ElementAt(i).StudentID;
                        worksheet.Cells[i + 5, 3].Value = allTopics.ElementAt(i).StudentName;
                        worksheet.Cells[i + 5, 4].Value = allTopics.ElementAt(i).TeacherName;
                        worksheet.Cells[i + 5, 5].Value = allTopics.ElementAt(i).Title;
                        worksheet.Cells[i + 5, 6].Value = allTopics.ElementAt(i).Description;
                        worksheet.Cells[i + 5, 7].Value = allTopics.ElementAt(i).ClassID;

                        // Đặt font chữ cho các ô dữ liệu
                        for (int j = 1; j <= 7; j++)
                        {
                            worksheet.Cells[i + 5, j].Style.Font.Size = 13;
                            worksheet.Cells[i + 5, j].Style.Font.Name = "Times New Roman";
                            // Thêm viền cho ô
                            worksheet.Cells[i + 5, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                    }

                    // Tự động điều chỉnh chiều rộng các cột
                    worksheet.Cells.AutoFitColumns();

                    // Tạo luồng dữ liệu để tải xuống
                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    // Trả về file Excel
                    var fileName = $"Topics_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }




        [HttpGet("TopicID")]
        public IActionResult GetStudenId(int TopicID)
        {
            if (!_topicRepository.TopicExists(TopicID))
                return NotFound();

            var Topic = _mapper.Map<TopicDTO>(_topicRepository.GetTopic(TopicID));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(Topic);
        }

        /*    [HttpGet("/Student/{StudentID}")]
            public IActionResult GetStudentByTopic(string StudentID)
            {
                var Topic = _mapper.Map<TopicDTO>(
                    _topicRepository.GetStudentByTopic(StudentID));

                if (!ModelState.IsValid)
                    return BadRequest();

                return Ok(Topic);
            } 

            [HttpGet("/teacher/{TeacherID}")]
            public IActionResult GetTeacherByTopic(int TeacherID)
            {
                var Topic = _mapper.Map<TopicDTO>(
                    _topicRepository.GetTeacherByTopic(TeacherID));

                if (!ModelState.IsValid)
                    return BadRequest();

                return Ok(Topic);
            }

            [HttpGet("/TopicType/{TopicTypeID}")]
            public IActionResult GetTopicTypeByTopic(int TopicTypeID)
            {
                var Topic = _mapper.Map<TopicDTO>(
                    _topicRepository.GetTopicTypeByTopic(TopicTypeID));

                if (!ModelState.IsValid)
                    return BadRequest();

                return Ok(Topic);
            }*/

            [HttpPost("Create")]
            public async Task<IActionResult> CreateTopic([FromBody] TopicRequest topicDTO)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Lấy thông tin StudentID và StatusTopic
                        var studentInfo = await _context.Students
                            .Where(s => s.StudentID == topicDTO.StudentID)
                            .Select(s => new { s.StudentID, s.StatusTopic })
                            .FirstOrDefaultAsync();

                        if (studentInfo == null)
                        {
                            return BadRequest("Sinh viên không tồn tại.");
                        }
                        var topicInfo = await _context.Topics
                        .Where(s => s.Title == topicDTO.Title)
                        .FirstOrDefaultAsync();

                    if (topicInfo == null)
                    {
                        return BadRequest(" Đề tài không tồn tại.");

                    }
                    // Tạo đề tài mới
                    var topic = new Topic
                        {
                            Title = topicDTO.Title,
                            Description = topicDTO.Description,
                            CreatedDate = DateTime.Now,
                            CreatedUser = "API",
                            status = 1,
                            StudentID = topicDTO.StudentID,
                            TeacherID = topicDTO.TeacherID,
                            FieldID = topicDTO.FieldID,
                            RegistrationDate = DateTime.Now.Year,
                        };

                        _context.Topics.Add(topic);
                        await _context.SaveChangesAsync(); // Lưu để có ID của topic mới

                    // Lấy thực thể Student đầy đủ để cập nhật StatusTopic
                    var student = await _context.Students
             .FirstOrDefaultAsync(s => s.StudentID == topicDTO.StudentID);

                    if (student == null)
                    {
                        return BadRequest("Student not found.");
                    }


                    if (student != null)
                        {
                            // Cập nhật StatusTopic
                            student.StatusTopic = 2;
                            _context.Students.Update(student);
                            await _context.SaveChangesAsync(); // Lưu thay đổi của sinh viên
                        }

                        await transaction.CommitAsync(); // Lưu thay đổi vào database

                        return Ok("Topic created and student status updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(); // Nếu có lỗi, rollback giao dịch
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }

        [HttpPut("api/UpdateStudentStatus/{studentID}/{newStatus}")]
        public IActionResult UpdateStudentStatus(string studentID, int newStatus)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentID == studentID);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            student.StatusTopic = newStatus;
            _context.SaveChanges();

            return Ok("Student status updated successfully.");
        }


        [HttpPut("api/UpdateTopic")]
        public IActionResult UpdateTopic(TopicRequest topicDto)
        {
            var topic = _context.Topics.FirstOrDefault(t => t.StudentID == topicDto.StudentID);
            if (topic == null)
            {
                return NotFound("Topic not found.");
            }

            topic.Title = topicDto.Title;
            topic.Description = topicDto.Description;
            topic.TeacherID = topicDto.TeacherID;
            topic.FieldID = topicDto.FieldID;
            topic.status = 1;

            var student = _context.Students.FirstOrDefault(s => s.StudentID == topicDto.StudentID);
            if (student != null)
            {
                student.StatusTopic = 2;
            }

            _context.SaveChanges();

            return Ok("Topic updated successfully.");
        }

       
        [HttpPut("Agree/{topicID}")]
        public async Task<IActionResult> Agree(int topicID)
        {
            try
            {
                // Find the topic
                var topic = await _context.Topics.FindAsync(topicID);
                if (topic == null)
                {
                    return NotFound("Topic not found");
                }

                // Update the status of the topic
                topic.status = 2; // Assuming 2 means "Agreed"
                
                // Find the student associated with the topic
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.StudentID == topic.StudentID);

                if (student == null)
                {
                    return NotFound("Student not found");
                }

                // Update the StatusTopic field of the student
                student.StatusTopic = 2; // Assuming 2 means "Agreed"
                student.StatusProgess = 1; // Assuming 2 means "Agreed"

                // Save changes to the database
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
                var topic = await _context.Topics.FindAsync(topicID);
                if (topic == null)
                {
                    return NotFound("Topic not found");
                }

                // Update the status of the topic
                topic.status = 3; // Assuming 3 means "Disagreed"

                // Find the student associated with the topic
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.StudentID == topic.StudentID);

                if (student == null)
                {
                    return NotFound("Student not found");
                }

                // Update the StatusTopic field of the student
                student.StatusTopic = 3; // Assuming 3 means "Disagreed"

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("AdminAgree/{topicID}")]
        public async Task<IActionResult> AdminAgree(int topicID)
        {
            try
            {
                // Find the topic
                var topic = await _context.Topics.FindAsync(topicID);
                if (topic == null)
                {
                    return NotFound("Topic not found");
                }

                // Update the status of the topic
                topic.status = 4; // Assuming 4 means "Admin Approved"

                // Find the student associated with the topic
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.StudentID == topic.StudentID);

                if (student == null)
                {
                    return NotFound("Student not found");
                }

                // Update the StatusTopic field of the student
                student.StatusTopic = 2; // Assuming 2 means "Admin Approved"

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetTopicsByTeacher/{teacherId}")]
        public async Task<IActionResult> GetTopicsByTeacher(int teacherId)
        {
            var topics = await _context.Topics
                .Where(t => t.TeacherID == teacherId)
                .Select(t => new TopicRequestByTeacher
                {
                    ID = t.ID,
                    Title=t.Title,
                    StudentID = t.StudentID,
                    ClassID = t.Student.ClassID,
                    Status = t.status,
                    Description = t.Description,
                    StudentName = t.Student.Name,
                    StudentClass = t.Student.Class.ClassName // Assuming Class has a ClassName property
                 })
        .ToListAsync();

            if (topics == null || !topics.Any())
            {
                return NotFound("No topics found for the specified teacher.");
            }

            return Ok(topics);
        }
        [HttpGet("GetTopicsByTeacherNull/{teacherId}")]
        public async Task<IActionResult> GetTopicsByTeacherNull(int teacherId)
        {
            var topics = await _context.Topics
                .Where(t => t.TeacherID == teacherId && t.StudentID == null && t.status == 6)
                .Select(t => new TopicRequestByTeacherNull
                {
                    ID = t.ID,
                    TeacherName = t.Teacher.Name,
                    Title = t.Title,
                    Description = t.Description,
                })
        .ToListAsync();

            if (topics == null || !topics.Any())
            {
                return NotFound("Không có đề tài nào");
            }

            return Ok(topics);
        }
        [HttpPut("UpdateTopic")]
        public async Task<IActionResult> UpdateTopic(int id, [FromBody] UpdateTopicRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound("Đề tài không tìm thấy");
            }

            // Update the topic with new values
            topic.Title = request.Title;
            topic.Description = request.Description;
            topic.FieldID = request.FieldID;

            // Save changes to the database
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();

            return Ok("Đề tài đã được cập nhật thành công");
        }


        [HttpGet("/GetTopicStudent/{studentID}")]
        public IActionResult GetStudentTopics(string studentID)
        {
            // Query to get student's topics with related teacher and field info
            var topics = _context.Topics
                                .Include(t => t.Student)
                                .Include(t => t.Teacher)
                                .Include(t => t.Field)
                                .Where(t => t.StudentID == studentID)
                                .Select(t => new StudentTopicRequest
                                {
                                    StudentID = t.StudentID,
                                    StudentName = t.Student.Name, // Assuming Student has a Name property
                                    Title = t.Title,
                                    Description = t.Description,
                                    Status = t.status,
                                    TeacherID = t.TeacherID,
                                    TeacherName = t.Teacher.Name, // Assuming Teacher has a Name property
                                    FieldID = t.FieldID,
                                    FieldName = t.Field.FieldName // Assuming Field has a FieldName property
                                })
                                .ToList();

            if (topics == null || topics.Count == 0)
            {
                return NotFound();
            }

            return Ok(topics);
        }
        [HttpPost("AddBasic")]
        public async Task<IActionResult> AddBasicTopic([FromBody] BasicTopicRequest basicTopicDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var topic = new Topic
            {
                Title = basicTopicDto.Title,
                Description = basicTopicDto.Description,
                CreatedDate = DateTime.Now,
                CreatedUser = "Admin",
                FieldID = basicTopicDto.FieldID,
                RegistrationDate = DateTime.Now.Year
                
            };

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTopicById), new { id = topic.ID }, topic);
        }

        // Add a new topic with all properties
        [HttpPost("AddFull")]
        public async Task<IActionResult> AddFullTopic([FromBody] FullTopicRequest fullTopicDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check for existing topic with the same title (case-insensitive)
            bool topicExists = await _context.Topics
                .AnyAsync(t => t.Title.ToLower() == fullTopicDto.Title.ToLower());

            if (topicExists)
            {
                return Conflict("Đề tài với tiêu đề này đã tồn tại.");
            }

            var topic = new Topic
            {
                Title = fullTopicDto.Title,
                Description = fullTopicDto.Description,
                CreatedDate = DateTime.Now,
                CreatedUser = "Admin",
                status = 4,
                StudentID = fullTopicDto.StudentID,
                TeacherID = fullTopicDto.TeacherID,
                FieldID = fullTopicDto.FieldID,
                RegistrationDate = DateTime.Now.Year
            };

            _context.Topics.Add(topic);

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentID == topic.StudentID);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            student.StatusTopic = 2; // Assuming 2 means "Disagreed"
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTopicById), new { id = topic.ID }, topic);
        }


        // Update an existing topic
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(int id, [FromBody] FullTopicRequest fullTopicDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            topic.Title = fullTopicDto.Title;
            topic.Description = fullTopicDto.Description;
            topic.CreatedUser ="admin";
            topic.status = 4;
            topic.StudentID = fullTopicDto.StudentID;
            topic.TeacherID = fullTopicDto.TeacherID;
            topic.FieldID = fullTopicDto.FieldID;
            topic.RegistrationDate = DateTime.Now.Year;

            _context.Entry(topic).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Delete a topic
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Get a topic by ID (for CreatedAtAction to work)
        [HttpGet("{id}")]
        public async Task<ActionResult<Topic>> GetTopicById(int id)
        {
            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
            {
                return NotFound();
            }

            return topic;
        }
        [HttpPost("AssignStudentAndTeacherToTopic")]
        public async Task<IActionResult> AssignStudentAndTeacherToTopic([FromBody] TopicAssignmentDTO topicAssignmentDTO)
        {
            try
            {
                // Validate the input
                if (topicAssignmentDTO == null || string.IsNullOrEmpty(topicAssignmentDTO.StudentID) || topicAssignmentDTO.TeacherID <= 0)
                {
                    return BadRequest("Invalid input data.");
                }

                // Check if the student is already assigned to a topic
                var existingStudentAssignment = await _context.Topics
                    .AnyAsync(t => t.StudentID == topicAssignmentDTO.StudentID);

                if (existingStudentAssignment)
                {
                    return BadRequest("Student is already assigned to a topic.");
                }

                // Create a new topic or update an existing topic
                var newTopic = new Topic
                {
                    StudentID = topicAssignmentDTO.StudentID,
                    TeacherID = topicAssignmentDTO.TeacherID,
                    status = 5,  // Set the status to 5 (just assigned)
                   
                };

                _context.Topics.Add(newTopic);
                await _context.SaveChangesAsync();

                return Ok("Student and teacher have been assigned to a topic.");
            }
            catch (Exception ex)
            {
                // Log the detailed exception
                var detailedError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Internal server error: {detailedError}");
            }

        }

        [HttpGet("availabletopics")]
        public IActionResult TestAvailableTopics([FromQuery] int teacherId)
        {
            var sql = @"SELECT ID, Title, Description
                FROM tblTopics
                WHERE TeacherID = @TeacherID AND StudentID IS NULL";

            var topics = _context.Topics.FromSqlRaw(sql, new SqlParameter("@TeacherID", teacherId))
                .Select(t => new
                {
                    t.ID,
                    t.Title,
                    t.Description
                })
                .ToList();

            return Ok(topics);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignTopic([FromBody] AssignTopicDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve the topic where StudentID is NULL
            var topic = await _context.Topics
                .Where(t => t.ID == dto.TopicID && t.StudentID == null)
                .Select(t => new { t.ID, t.Title })
                .SingleOrDefaultAsync();

            if (topic == null)
            {
                return NotFound(new { message = "Không tìm thấy đề tài phù hợp." });
            }

            // Check if the student exists
            var studentExists = await _context.Students
                .AnyAsync(s => s.StudentID == dto.StudentID);

            if (!studentExists)
            {
                return NotFound(new { message = "Sinh viên không tồn tại." });
            }

            // Retrieve the topic entity to update
            var topicEntity = await _context.Topics.FindAsync(topic.ID);
            if (topicEntity == null)
            {
                return NotFound(new { message = "Đề tài không tồn tại." });
            }

            // Update the topic entity
            topicEntity.StudentID = dto.StudentID;
            topicEntity.status = 4; // Update status if needed
            topicEntity.RegistrationDate = DateTime.UtcNow.Year;

            // Perform partial update for student
            var studentIdParam = new SqlParameter("@StudentID", dto.StudentID);
            var statusTopicParam = new SqlParameter("@StatusTopic", 2);

            // Execute raw SQL to update the StatusTopic
            await _context.Database.ExecuteSqlRawAsync(
                "UPDATE tblStudent SET StatusTopic = @StatusTopic WHERE StudentID = @StudentID",
                statusTopicParam,
                studentIdParam
            );

            // Update the topic entity in the database
            _context.Topics.Update(topicEntity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lưu dữ liệu.", error = dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lưu dữ liệu.", error = ex.Message });
            }

            return Ok(new { message = $"Sinh viên đã được phân công vào đề tài {topicEntity.Title}." });
        }


        [HttpPost("registerTeacher")]
        public async Task<IActionResult> RegisterTopic([FromBody] RegisterTopicTeacherDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var teacher = await _context.Teachers.FindAsync(dto.TeacherID);

            // Log if the teacher is not found
            if (teacher == null)
            {
                return NotFound(new { message = "Giảng viên không tồn tại." });
            }

            var topic = new Topic
            {
                Title = dto.Title,
                Description = dto.Description,
                CreatedDate = DateTime.UtcNow,
                CreatedUser = dto.TeacherID.ToString(),
                TeacherID = dto.TeacherID,
                status = 6, // Assuming 1 means "Pending",
                FieldID = dto.FieldID
            };

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đề tài đã được đăng ký thành công." });
        }

    }
}
