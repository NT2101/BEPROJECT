using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO.Request;
using Project.Models;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DataContext _context;

        public AuthenticationController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        /* public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
         {
             if (loginRequest == null)
                 return BadRequest("Invalid client request");

             var account = await _context.Accounts
                 .Include(a => a.Role)
                 .FirstOrDefaultAsync(a => a.Name == loginRequest.Username && a.Password == loginRequest.Password);

             if (account == null)
                 return Unauthorized("Invalid username or password");

             // Store user info in session
             HttpContext.Session.SetInt32("UserID", account.ID);
             HttpContext.Session.SetInt32("RoleID", account.RoleID);

             var userInfo = new
             {
                 UserID = account.ID,
                 Username = account.Name,
                 RoleID = account.RoleID,
                 RoleName = account.Role.RoleName,
                 CreatedDate = account.CreatedDate.HasValue ? account.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                 ModifiedDate = account.ModifiedDate.HasValue ? account.ModifiedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                 TeacherInfo = account.RoleID == 2 ? await _context.Teachers.FirstOrDefaultAsync(t => t.AccountID == account.ID) : null,
                 StudentInfo = account.RoleID == 1 ? await _context.Students.FirstOrDefaultAsync(s => s.AccountID == account.ID) : null
             };

             return Ok(userInfo);
         }*/
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Find the account based on username and password
                var account = await _context.Accounts
                    .SingleOrDefaultAsync(a => a.Name == model.Username && a.Password == model.Password);

                if (account == null)
                {
                    return NotFound("Invalid username or password");
                }

                // Determine the role of the account
                if (account.RoleID == 2) // Teacher
                {
                    // Load teacher information
                    var teacher = await _context.Teachers
                        .Where(t => t.AccountID == account.ID)
                        .Select(t => new { t.TeacherID, t.Name, t.Status })
                        .FirstOrDefaultAsync();

                    if (teacher == null)
                    {
                        return NotFound("Teacher profile not found");
                    }

                    // Return only TeacherID along with account details
                    return Ok(new
                    {
                        Account = account,
                        TeacherInfo = teacher,
                        Name = teacher.Name,
                    });
                }
                else if (account.RoleID == 1) // Student
                {
                    // Load student information, handling potential null values
                    var student = await _context.Students
                        .Where(s => s.AccountID == account.ID)
                        .Select(s => new
                        {
                            s.StudentID,
                            s.Name,
                            Status = (int?)s.Status ?? 0, // Use null-coalescing operator to handle null values
                            StatusTopic = (int?)s.StatusTopic ?? 0,
                            StatusProgess = (int?)s.StatusProgess ?? 0
                        })
                        .FirstOrDefaultAsync();

                    if (student == null)
                    {
                        return NotFound("Student profile not found");
                    }

                    // Return StudentID and status fields along with account details
                    return Ok(new
                    {
                        Account = account,
                        StudentInfo = student,
                        Name = student.Name,
                    });
                }
                else if (account.RoleID == 3)
                {
                    // If the RoleID is neither 1 (Student) nor 2 (Teacher)
                    return Ok(new { Account = account ,
                                    Name =account.Name});
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("User logged out successfully");
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var roleId = HttpContext.Session.GetInt32("RoleID");

            if (userId == null || roleId == null)
                return Unauthorized("No user logged in");

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.ID == userId);

            if (account == null)
                return NotFound("User not found");

            var userInfo = new
            {
                UserID = account.ID,
                Username = account.Name,
                RoleID = account.RoleID,
                RoleName = account.Role.RoleName,
                TeacherInfo = account.RoleID == 0 ? await _context.Teachers.FirstOrDefaultAsync(t => t.AccountID == account.ID) : null,
                StudentInfo = account.RoleID == 1 ? await _context.Students.FirstOrDefaultAsync(s => s.AccountID == account.ID) : null
            };

            return Ok(userInfo);
        }
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = _context.Accounts.FirstOrDefault(a => a.ID == dto.AccountID);

            if (account == null)
            {
                return NotFound("Account not found.");
            }

            if (account.Password != dto.OldPassword)
            {
                return BadRequest("Old password is incorrect.");
            }

            account.Password = dto.NewPassword;
            account.ModifiedDate = DateTime.Now;
            account.ModifiedUser = "System"; // You might want to set this to the currently logged-in user

            _context.SaveChanges();

            return Ok("Password changed successfully.");
        }
    }
}
