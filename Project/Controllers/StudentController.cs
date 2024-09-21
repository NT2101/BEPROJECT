using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Project.Data;
using Project.DTO;
using Project.DTO.Request;
using Project.Interfaces;
using Project.Models;
using Project.Repositories;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;


namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;
        private readonly DataContext _context;


        public StudentController(IStudentRepository studentRepository, IMapper mapper, DataContext context)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<StudentDTO>))]
        public IActionResult GetAllStudents()
        {
            try
            {
                var studentDTOs = _context.Students
                    .Select(s => new Student
                    {
                        StudentID = s.StudentID,
                        Name = s.Name ?? "Chưa có",
                        Dob = s.Dob,
                        Sex = s.Sex,
                        Address = s.Address ?? "Chưa có",
                        PhoneNumber = s.PhoneNumber ?? "Chưa có",
                        EmailAddress = s.EmailAddress ?? "Chưa có",
                        Country = s.Country ?? "Chưa có",
                        ClassID = s.ClassID,
                    })
                    .ToList();

                return Ok(studentDTOs);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("WithoutTopics")]
        public async Task<ActionResult<IEnumerable<StudentWithoutTopicRequest>>> GetStudentsWithoutTopics()
        {
            var studentsWithoutTopics = await _context.Students
                .Where(student => !_context.Topics.Any(topic => topic.StudentID == student.StudentID))
                .Select(student => new StudentWithoutTopicRequest
                {
                    StudentID = student.StudentID,
                    Name = student.Name,
           
                })
                .ToListAsync();

            return Ok(studentsWithoutTopics);
        }
        [HttpGet("export/students")]
        [ProducesResponseType(200, Type = typeof(FileResult))]
        public IActionResult ExportStudentsToExcel()
        {
            try
            {
                // Get student data
                var students = _context.Students
                    .Select(s => new StudentExport
                    {
                        StudentID = s.StudentID,
                        Name = s.Name ?? "Chưa có",
                        Dob = s.Dob,
                        Sex = s.Sex == 0 ? "Nam" : "Nữ",
                        Address = s.Address ?? "Chưa có",
                        PhoneNumber = s.PhoneNumber ?? "Chưa có",
                        EmailAddress = s.EmailAddress ?? "Chưa có",
                        Country = s.Country ?? "Chưa có",
                        ClassID = s.ClassID ?? "Chưa có"
                    })
                    .ToList();

                // Create a new Excel package
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Danh sách sinh viên");

                    // Add title
                    worksheet.Cells[1, 1].Value = "Danh sách sinh viên";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.Font.Size = 15;
                    worksheet.Cells[1, 1].Style.Font.Name = "Times New Roman";
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center horizontally
                    worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Center vertically
                    worksheet.Cells[1, 1, 3, 9].Merge = true; // Merge A1, A2, A3 across columns 1 to 9
                    worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid; // Set pattern type
                    worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.White);

                    // The merged cell will automatically create space below the title


                    // Add headers
                    var headers = new[] { "Mã Sinh Viên", "Tên", "Ngày Sinh", "Giới Tính", "Địa Chỉ", "Số Điện Thoại", "Email", "Quốc Gia", "Lớp" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[4, i + 1].Value = headers[i];
                        worksheet.Cells[4, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[4, i + 1].Style.Font.Size = 13;
                        worksheet.Cells[4, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[4, i + 1].Style.Font.Name = "Times New Roman";

                        // Set the background color to white
                        worksheet.Cells[4, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[4, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255)); // White background

                        // Set font color to a specific color, e.g., black
                        worksheet.Cells[4, i + 1].Style.Font.Color.SetColor(Color.Black); // Explicitly set text color to black
                    }


                    // Add data rows
                    for (int i = 0; i < students.Count; i++)
                    {
                        var student = students[i];
                        worksheet.Cells[i + 5, 1].Value = student.StudentID;
                        worksheet.Cells[i + 5, 2].Value = student.Name;
                        worksheet.Cells[i + 5, 3].Value = student.Dob.ToString("dd/MM/yyyy");
                        worksheet.Cells[i + 5, 4].Value = student.Sex;
                        worksheet.Cells[i + 5, 5].Value = student.Address;
                        worksheet.Cells[i + 5, 6].Value = student.PhoneNumber;
                        worksheet.Cells[i + 5, 7].Value = student.EmailAddress;
                        worksheet.Cells[i + 5, 8].Value = student.Country;
                        worksheet.Cells[i + 5, 9].Value = student.ClassID;

                        // Set data cells to left alignment and font style
                        for (int j = 1; j <= 9; j++)
                        {
                            worksheet.Cells[i + 5, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[i + 5, j].Style.Font.Name = "Times New Roman";
                            worksheet.Cells[i + 5, j].Style.Font.Size = 12;
                            worksheet.Cells[i + 5, j].Style.Fill.PatternType = ExcelFillStyle.Solid; // Set pattern type
                            worksheet.Cells[i + 5, j].Style.Fill.BackgroundColor.SetColor(Color.White);
                        }
                    }

                    // Format the entire table with borders
                    var range = worksheet.Cells[4, 1, students.Count + 4, 9];
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    // Create a table for the data range
                    var table = worksheet.Tables.Add(range, "StudentTable");
                    table.TableStyle = TableStyles.Medium9;

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    // Prepare the file for download
                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    var fileName = "DanhSachSinhVien.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }



        [HttpGet("StudentID")]

        public IActionResult GetStudenId(string StudentID)
        {
            if (!_studentRepository.StudentExists(StudentID))
                return NotFound();

            var student = _mapper.Map<StudentDTO>(_studentRepository.GetStudent(StudentID));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(student);
        }
        [HttpGet("download-sample")]
        public IActionResult DownloadSample()
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sample");

                    // Đặt tiêu đề cột
                    worksheet.Cells[1, 1].Value = "Mã sinh viên";
                    worksheet.Cells[1, 2].Value = "Tên sinh viên";
                    worksheet.Cells[1, 3].Value = "Ngày sinh";
                    worksheet.Cells[1, 4].Value = "Mã lớp";

                    // Thêm dữ liệu ví dụ
                    worksheet.Cells[2, 1].Value = "S12345";
                    worksheet.Cells[2, 2].Value = "Nguyen Van A";
                    worksheet.Cells[2, 3].Value = "01/01/2000";
                    worksheet.Cells[2, 4].Value = "L01";

                    worksheet.Cells[3, 1].Value = "S12346";
                    worksheet.Cells[3, 2].Value = "Tran Thi B";
                    worksheet.Cells[3, 3].Value = "15/05/1999";
                    worksheet.Cells[3, 4].Value = "L02";

                    worksheet.Cells[4, 1].Value = "S12347";
                    worksheet.Cells[4, 2].Value = "Le Van C";
                    worksheet.Cells[4, 3].Value = "20/12/2001";
                    worksheet.Cells[4, 4].Value = "L01";

                    // Định dạng cột
                    worksheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
                    worksheet.Cells[1, 1, 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, 1, 1, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, 1, 1, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, 1, 1, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    // Tạo luồng dữ liệu để tải xuống
                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    // Trả về file Excel
                    var fileName = "SampleStudentImport.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpPost("import-students")]
        public async Task<IActionResult> ImportStudentsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            List<Student> students = new List<Student>();
            List<Account> accounts = new List<Account>();

            try
            {
                using (var package = new ExcelPackage(file.OpenReadStream()))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Đọc bảng tính đầu tiên

                    // Bỏ qua dòng tiêu đề
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var studentID = worksheet.Cells[row, 1].Text; // Mã sinh viên
                        var name = worksheet.Cells[row, 2].Text; // Tên
                        var dobString = worksheet.Cells[row, 3].Text; // Ngày sinh
                        var classID = worksheet.Cells[row, 4].Text; // Mã lớp

                        if (string.IsNullOrEmpty(studentID) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(dobString) || string.IsNullOrEmpty(classID))
                        {
                            continue; // Bỏ qua các dòng dữ liệu thiếu
                        }

                        if (!DateTime.TryParse(dobString, out var dob))
                        {
                            continue; // Bỏ qua các dòng với ngày sinh không hợp lệ
                        }

                        // Tạo một đối tượng Account mới
                        var account = new Account
                        {
                            Name = studentID, // Tên tài khoản là mã sinh viên
                            Password = dob.ToString("ddMMyyyy"), // Password là ngày sinh theo định dạng yyyyMMdd
                            RoleID = 1, // RoleID mặc định là 1 (sinh viên)
                            Status = 1, // Trạng thái mặc định
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            CreatedUser = "API",
                            ModifiedUser = "API"
                        };

                        accounts.Add(account);

                        // Tạo một đối tượng Student mới
                        var student = new Student
                        {
                            StudentID = studentID,
                            Name = name,
                            Dob = dob,
                            ClassID = classID,
                            AccountID = account.ID, // Đặt AccountID sẽ được gán sau khi lưu
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            CreatedUser = "API",
                            ModifiedUser = "API",
                            Status = 1, // Giá trị mặc định
                            StatusTopic = 0, // Giá trị mặc định
                            StatusProgess = 0 // Giá trị mặc định
                        };

                        students.Add(student);
                    }
                }

                // Lưu dữ liệu vào cơ sở dữ liệu
                // Thêm tài khoản vào cơ sở dữ liệu trước
                _context.Accounts.AddRange(accounts);
                await _context.SaveChangesAsync();

                // Cập nhật AccountID cho các sinh viên sau khi tài khoản đã được lưu
                foreach (var student in students)
                {
                    var account = accounts.FirstOrDefault(a => a.Name == student.StudentID);
                    if (account != null)
                    {
                        student.AccountID = account.ID;
                    }
                }

                // Thêm sinh viên vào cơ sở dữ liệu
                _context.Students.AddRange(students);
                await _context.SaveChangesAsync();

                return Ok(new { Count = students.Count });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("students/class")]
        public async Task<IActionResult> GetStudents([FromQuery] string classId = null)
        {
            try
            {
                IQueryable<Student> query = _context.Students;

                if (!string.IsNullOrEmpty(classId))
                {
                    query = query.Where(s => s.ClassID == classId);
                }

                var students = await query
                    .Select(s => new Student
                    {
                        StudentID = s.StudentID,
                        Name = s.Name ?? "Chưa có",
                        Dob = s.Dob,
                        Sex = s.Sex,
                        Address = s.Address ?? "Chưa có",
                        PhoneNumber = s.PhoneNumber ?? "Chưa có",
                        EmailAddress = s.EmailAddress ?? "Chưa có",
                        Country = s.Country ?? "Chưa có",
                        ClassID = s.ClassID,
                    })
                    .ToListAsync();

                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromForm] CreateStudentAdmin studentDto)
        {
            // Create an execution strategy
            var strategy = _context.Database.CreateExecutionStrategy();

            // Execute all operations in a retriable unit
            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Kiểm tra xem mã sinh viên đã tồn tại hay chưa
                        var existingStudent = await _context.Students
                            .FirstOrDefaultAsync(s => s.StudentID == studentDto.StudentID);

                        if (existingStudent != null)
                        {
                            return BadRequest("Student with the same ID already exists.");
                        }

                        // Tạo một account mới
                        var account = new Account
                        {
                            Name = studentDto.StudentID, // Sử dụng StudentID làm Name của account
                            Password = studentDto.Dob.ToString("ddMMyyyy"), // Password là MMyyyy của Dob
                            RoleID = 1, // Giả sử RoleID mặc định là 1
                            Status = 1, // Giả sử Status mặc định là 1
                            CreatedDate = DateTime.Now,
                            CreatedUser = "API",
                            ModifiedDate = DateTime.Now,
                            ModifiedUser = "API"
                        };

                        // Thêm account vào DbContext
                        _context.Accounts.Add(account);
                        await _context.SaveChangesAsync(); // Lưu để có được ID của account mới được tạo

                        // Tạo một student và liên kết với account vừa tạo
                        var student = new Student
                        {
                            StudentID = studentDto.StudentID,
                            Name = studentDto.Name,
                            Dob = studentDto.Dob,
                            ClassID = studentDto.ClassID,
                            AccountID = account.ID, // Sử dụng ID của account vừa được tạo
                            Status = 1,
                            StatusTopic = 0,
                            StatusProgess = 1,
                            CreatedDate = DateTime.Now,
                            CreatedUser = "API",
                            ModifiedDate = DateTime.Now,
                            ModifiedUser = "API",
                        };

                        // Thêm student vào DbContext
                        _context.Students.Add(student);
                        await _context.SaveChangesAsync(); // Lưu để hoàn thành việc thêm student

                        await transaction.CommitAsync(); // Lưu thay đổi vào database

                        // Tạo phản hồi
                        return Ok("Student created successfully.");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(); // Nếu có lỗi, rollback giao dịch
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            });
        }

        [HttpDelete("{studentID}")]
        public async Task<IActionResult> DeleteStudent(string studentID)
        {
            try
            {
                // Xây dựng câu lệnh SQL để lấy AccountID của sinh viên
                var sqlGetAccountID = "SELECT AccountID FROM tblStudent WHERE StudentID = @StudentID";
                var studentAccountId = await _context.Database.ExecuteSqlRawAsync(sqlGetAccountID,
                    new SqlParameter("@StudentID", studentID));

                if (studentAccountId == null)
                {
                    return NotFound($"Student with ID {studentID} not found.");
                }

                // Xóa tài khoản của sinh viên
                var sqlDeleteAccount = "DELETE FROM tblAccount WHERE ID = @AccountID";
                await _context.Database.ExecuteSqlRawAsync(sqlDeleteAccount,
                    new SqlParameter("@AccountID", studentAccountId));

                // Xóa sinh viên
                var sqlDeleteStudent = "DELETE FROM tblStudent WHERE StudentID = @StudentID";
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sqlDeleteStudent,
                    new SqlParameter("@StudentID", studentID));

                if (rowsAffected == 0)
                {
                    return NotFound($"Student with ID {studentID} not found.");
                }

                return NoContent(); // Trả về HTTP 204 No Content nếu thành công
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return StatusCode(500, $"An error occurred while deleting the student: {ex.Message}");
            }
        }


        /*[HttpGet("check-registration/{studentID}")]
        public async Task<IActionResult> CheckStudentRegistration(string studentID)
        {
            var isRegistered = await _studentRepository.IsStudentRegisteredForTopic(studentID);
            return Ok(new { registered = isRegistered });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateStudent(string id, [FromForm] StudentUpdateDTO studentUpdateDto, IFormFile profilePicture)
        {
            var student = await _context.Students
                                         .Include(s => s.account) // Ensure the account is included
                                         .FirstOrDefaultAsync(s => s.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin sinh viên
            student.Name = studentUpdateDto.Name;
            student.Dob = studentUpdateDto.Dob;
            student.Sex = studentUpdateDto.Sex;
            student.Address = studentUpdateDto.Address;
            student.PhoneNumber = studentUpdateDto.PhoneNumber;
            student.EmailAddress = studentUpdateDto.EmailAddress;
            student.Country = studentUpdateDto.Country;
            student.Status = 2;
            student.ModifiedDate = DateTime.UtcNow;
            student.ModifiedUser = "system"; // Update this as needed

            // Cập nhật mật khẩu nếu có
            if (!string.IsNullOrEmpty(studentUpdateDto.Password))
            {
                student.account.Password = studentUpdateDto.Password;
            }

            // Xử lý ảnh nếu có
            if (profilePicture != null)
            {
                var filePath = Path.Combine(@"E:\Process", profilePicture.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }
                student.ProfilePictureUrl = filePath; // Cập nhật đường dẫn ảnh
            }

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }*/
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] StudentUpdateDTO studentUpdateDto)
        {
            // Tìm sinh viên theo ID với các trường cần thiết
            var student = await _context.Students
                                         .Include(s => s.account)
                                         .Where(s => s.StudentID == id)
                                         .Select(s => new
                                         {
                                             s.StudentID,
                                             s.AccountID, // Bao gồm AccountID trong truy vấn
                                             s.Name,
                                             s.Dob,
                                             s.Sex,
                                             Address = s.Address ?? "Chưa có",
                                             PhoneNumber = s.PhoneNumber ?? "Chưa có",
                                             EmailAddress = s.EmailAddress ?? "Chưa có",
                                             Country = s.Country ?? "Chưa có",
                                             s.Status,
                                             s.StatusTopic,
                                             s.ModifiedDate,
                                             s.ModifiedUser,
                                             s.ClassID,
                                             s.account
                                         })
                                         .FirstOrDefaultAsync();

            // Nếu không tìm thấy sinh viên, trả về NotFound
            if (student == null)
            {
                return NotFound();
            }

            // Tạo đối tượng Student mới để cập nhật
            var studentToUpdate = new Student
            {
                StudentID = student.StudentID,
                AccountID = student.AccountID, // Đảm bảo sao chép AccountID
                Name = studentUpdateDto.Name ?? student.Name,
                Dob = studentUpdateDto.Dob != default(DateTime) ? studentUpdateDto.Dob : student.Dob,
                Sex = studentUpdateDto.Sex >= 0 ? studentUpdateDto.Sex : student.Sex,
                Address = studentUpdateDto.Address ?? student.Address,
                PhoneNumber = studentUpdateDto.PhoneNumber ?? student.PhoneNumber,
                EmailAddress = studentUpdateDto.EmailAddress ?? student.EmailAddress,
                Country = studentUpdateDto.Country ?? student.Country,
                Status = 2, // Ví dụ, có thể cần thiết lập giá trị Status mặc định
                StatusTopic = 1, // Ví dụ, có thể cần thiết lập giá trị StatusTopic mặc định
                ModifiedDate = DateTime.UtcNow,
                CreatedUser = "API",
                ModifiedUser = "system", // Cập nhật người dùng sửa đổi
                ClassID = student.ClassID,
            };

            // Cập nhật mật khẩu nếu có
            if (!string.IsNullOrEmpty(studentUpdateDto.Password))
            {
                if (student.account == null)
                {
                    studentToUpdate.account = new Account
                    {
                        Password = studentUpdateDto.Password
                        // Thiết lập các thuộc tính khác nếu cần
                    };
                }
                else
                {
                    student.account.Password = studentUpdateDto.Password;
                }
            }

            // Cập nhật thông tin sinh viên trong cơ sở dữ liệu
            _context.Students.Update(studentToUpdate);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpGet("GetStudentsNotAssignedToTopics")]
        public async Task<IActionResult> GetStudentsNotAssignedToTopics()
        {
            var studentsNotAssigned = await _context.Students
                .Where(s => !_context.Topics.Any(t => t.StudentID == s.StudentID))
                .Select(s => new Student
                {
                    StudentID = s.StudentID,
                    Name = s.Name ?? "Chưa có",
                    Dob = s.Dob,
                    Sex = s.Sex,
                    Address = s.Address ?? "Chưa có",
                    PhoneNumber = s.PhoneNumber ?? "Chưa có",
                    EmailAddress = s.EmailAddress ?? "Chưa có",
                    Country = s.Country ?? "Chưa có",
                    ClassID = s.ClassID,
                })
                .ToListAsync();

            return Ok(studentsNotAssigned);
        }
        [HttpGet("total-student-count")]
        public async Task<IActionResult> GetTotalStudentCount()
        {
            var totalCount = await _context.Students.CountAsync();
            return Ok(new { totalCount });
        }

    }
}
