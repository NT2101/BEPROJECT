using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Project.Data;
using Project.DTO;
using Project.DTO.Request;
using Project.Interfaces;
using Project.Models;
using System.Security.Cryptography;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : Controller
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public TeacherController(ITeacherRepository teacherRepository, IMapper mapper, DataContext context)
        {
            _teacherRepository = teacherRepository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TeacherDTO>))]
        public IActionResult GetAllTeachers()
        {
            try
            {
                var teacherDTOs = _context.Teachers
                    .Select(t => new TeacherDTO
                    {
                        TeacherID = t.TeacherID,
                        Name = t.Name ?? "Chưa có",  
                        Dob = t.Dob,
                        Sex = t.Sex, 
                        PhoneNumber = t.PhoneNumber ?? "Chưa có",
                        EmailAddress = t.EmailAddress ?? "Chưa có",  
                        Description = t.Description ?? "Chưa có",  
                        FacultyID = t.FacultyID ?? "Chưa có",
                        Status = t.Status
                    })
                    .ToList();

                return Ok(teacherDTOs);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost("import")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            List<Teacher> teachers = new List<Teacher>();

            try
            {
                using (var package = new ExcelPackage(file.OpenReadStream()))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Đọc worksheet đầu tiên

                    // Skip header row
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var name = worksheet.Cells[row, 1].Text; // Tên
                        var emailAddress = worksheet.Cells[row, 2].Text; // Email
                        var dobString = worksheet.Cells[row, 3].Text; // Ngày Sinh
                        var facultyCode = worksheet.Cells[row, 4].Text; // Khoa

                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(emailAddress) || string.IsNullOrEmpty(dobString) || string.IsNullOrEmpty(facultyCode))
                        {
                            continue; // Bỏ qua hàng không có dữ liệu cần thiết
                        }

                        if (!DateTime.TryParse(dobString, out var dob))
                        {
                            continue; // Bỏ qua hàng có ngày sinh không hợp lệ
                        }

                        // Kiểm tra xem FacultyID có tồn tại trong bảng tblFaculties không
                        var faculty = _context.Faculties.FirstOrDefault(f => f.ID == facultyCode); // Sử dụng Code hoặc ID tương ứng
                        if (faculty == null)
                        {
                            return BadRequest($"Faculty with Code {facultyCode} does not exist.");
                        }

                        // Tạo một tài khoản mới
                        var newAccount = new Account
                        {
                            Name = emailAddress, // Giả sử email được sử dụng làm tên tài khoản
                            Password = dob.ToString("ddMMyyyy"), // Sử dụng ngày sinh làm mật khẩu
                            Status = 1,
                            RoleID = 2,
                            CreatedUser="api",
                            ModifiedUser = "api",
                            ModifiedDate = DateTime.Now,
                            CreatedDate = DateTime.Now

                        };

                        _context.Accounts.Add(newAccount);
                        await _context.SaveChangesAsync(); // Lưu thay đổi để có AccountID

                        // Tạo một giáo viên mới
                        var teacher = new Teacher
                        {
                            Name = name,
                            EmailAddress = emailAddress,
                            Dob = dob,
                            Sex =1,
                            FacultyID = faculty.ID, // Sử dụng ID của faculty
                            AccountID = newAccount.ID, // Đặt AccountID
                            Status = 1, // Trạng thái mặc định
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                        ModifiedUser = "API",
                            CreatedUser = "API",

                        };

                        teachers.Add(teacher);
                    }
                }

                _context.Teachers.AddRange(teachers);
                await _context.SaveChangesAsync();
                return Ok(new { Count = teachers.Count });
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                return StatusCode(500, $"Internal server error: {ex.Message}. Inner exception: {innerException}");
            }
        }


        [HttpGet("{TeacherID}")]
        public IActionResult GetTeacherById(int TeacherID)
        {
            if (!_teacherRepository.TeacherExists(TeacherID))
                return NotFound();

            var teacherDTO = _teacherRepository.GetTeacher(TeacherID);

            if (teacherDTO == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(teacherDTO);
        }


        [HttpPost]
        public IActionResult CreateTeacher(TeacherRequest teacherDto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Kiểm tra FacultyID tồn tại
                    var faculty = _context.Faculties.Find(teacherDto.FacultyID);
                    if (faculty == null)
                    {
                        return BadRequest("FacultyID không tồn tại.");
                    }

                    // Tạo một account mới
                    var account = new Account
                    {
                        Name = teacherDto.EmailAddress,
                        Password = teacherDto.Dob.ToString("ddMMyyyy"), // Password là ddMMyyyy của Dob
                        RoleID = 2, // Giả sử RoleID mặc định là 2
                        Status = 1, // Giả sử Status mặc định là 1
                        CreatedDate = DateTime.Now,
                        CreatedUser = "API",
                        ModifiedDate = DateTime.Now,
                        ModifiedUser = "API"
                    };

                    // Thêm account vào DbContext
                    _context.Accounts.Add(account);
                    _context.SaveChanges(); // Lưu để có được ID của account mới được tạo

                    // Tạo một Teacher và liên kết với account vừa tạo
                    var teacher = new Teacher
                    {
                        Name = teacherDto.Name,
                        Dob = teacherDto.Dob,
                        Sex = teacherDto.Sex,
                        PhoneNumber = teacherDto.PhoneNumber,
                        EmailAddress = teacherDto.EmailAddress,
                        Description = teacherDto.Description,
                        AccountID = account.ID, // Sử dụng ID của account vừa được tạo
                        FacultyID = teacherDto.FacultyID, // Sử dụng FacultyID từ DTO
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        CreatedUser = "API",
                        ModifiedDate = DateTime.Now,
                        ModifiedUser = "API"
                    };

                    // Thêm Teacher vào DbContext
                    _context.Teachers.Add(teacher);
                    _context.SaveChanges(); // Lưu để hoàn thành việc thêm Teacher

                    transaction.Commit(); // Lưu thay đổi vào database

                    return Ok("Teacher created successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Nếu có lỗi, rollback giao dịch
                    return StatusCode(500, $"Internal server error: {ex}");
                }
            }
        }
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] TeacherUpdateDTO UpdateDto)
        {
            // Tìm sinh viên theo ID với các trường cần thiết
            var teacher = await _context.Teachers
                                         .Include(s => s.account)
                                         .Where(s => s.TeacherID == id)
                                         .Select(s => new
                                         {
                                             s.TeacherID,
                                             s.AccountID, // Bao gồm AccountID trong truy vấn
                                             s.Name,
                                             s.Dob,
                                             s.Sex,
                                             Description = s.Description ?? "Chưa có",
                                             PhoneNumber = s.PhoneNumber ?? "Chưa có",
                                             EmailAddress = s.EmailAddress ?? "Chưa có",
                                             s.Status,
                                             s.ModifiedDate,
                                             s.ModifiedUser,
                                             s.FacultyID,
                                             s.account
                                         })
                                         .FirstOrDefaultAsync();

            // Nếu không tìm thấy sinh viên, trả về NotFound
            if (teacher == null)
            {
                return NotFound();
            }

            // Tạo đối tượng Student mới để cập nhật
            var teacherToUpdate = new Teacher
            {
                TeacherID = teacher.TeacherID,
                AccountID = teacher.AccountID, // Đảm bảo sao chép AccountID
                Name = UpdateDto.Name ?? teacher.Name,
                Dob = UpdateDto.Dob != default(DateTime) ? UpdateDto.Dob : teacher.Dob,
                Sex = UpdateDto.Sex >= 0 ? UpdateDto.Sex : teacher.Sex,
                Description = UpdateDto.Description ?? teacher.Description,
                PhoneNumber = UpdateDto.PhoneNumber ?? teacher.PhoneNumber,
                EmailAddress = UpdateDto.EmailAddress ?? teacher.EmailAddress,
                Status = 2, // Ví dụ, có thể cần thiết lập giá trị Status mặc định
                ModifiedDate = DateTime.UtcNow,
                ModifiedUser = "system", // Cập nhật người dùng sửa đổi
                FacultyID = teacher.FacultyID,
            };

            // Cập nhật mật khẩu nếu có
            if (!string.IsNullOrEmpty(UpdateDto.Password))
            {
                if (teacher.account == null)
                {
                    teacherToUpdate.account = new Account
                    {
                        Password = UpdateDto.Password
                        // Thiết lập các thuộc tính khác nếu cần
                    };
                }
                else
                {
                    teacher.account.Password = UpdateDto.Password;
                }
            }

            // Cập nhật thông tin sinh viên trong cơ sở dữ liệu
            _context.Teachers.Update(teacherToUpdate);
            await _context.SaveChangesAsync();

            return NoContent();
        }




      /*  [HttpPut("{TeacherID}")]
        public IActionResult UpdateTeacher(int TeacherID, [FromBody] TeachersDTO updatedTeacherDto)
        {
            if (updatedTeacherDto == null)
                return BadRequest("Invalid data.");

            if (TeacherID != updatedTeacherDto.TeacherID)
                return BadRequest("Mismatched teacher ID.");

            var existingTeacher = _context.Teachers
                                          .Include(t => t.account) // Ensure the account is included
                                          .FirstOrDefault(t => t.TeacherID == TeacherID);

            if (existingTeacher == null)
                return NotFound("Teacher not found.");

            // Map properties from DTO to existing entity
            _mapper.Map(updatedTeacherDto, existingTeacher);

            try
            {
                if (!_teacherRepository.UpdateTeacher(existingTeacher))
                {
                    ModelState.AddModelError("", "Error occurred while updating the teacher.");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating teacher: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }
*/
        [HttpDelete("{TeacherID}")]
        public IActionResult DeleteTeacher(int TeacherID)
        {
            if (!_teacherRepository.TeacherExists(TeacherID))
                return NotFound("Teacher not found.");

            var teacherToDelete = _context.Teachers.FirstOrDefault(t => t.TeacherID == TeacherID);

            if (teacherToDelete == null)
                return NotFound("Teacher not found.");

            try
            {
                if (!_teacherRepository.DeleteTeacher(teacherToDelete))
                {
                    ModelState.AddModelError("", "Unable to delete teacher.");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting teacher: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        

    }

    [HttpGet("registrationRequests")]
        public ActionResult<IEnumerable<RegistrationRequest>> GetRegistrationRequests()
        {
            try
            {
                // Lấy danh sách các yêu cầu đăng ký hướng dẫn chờ xác nhận từ sinh viên
                var requests = _context.RegistrationRequests.Where(r => r.IsConfirmed == false).ToList();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy danh sách yêu cầu đăng ký hướng dẫn: {ex.Message}");
            }
        }

       
      

    }
}
