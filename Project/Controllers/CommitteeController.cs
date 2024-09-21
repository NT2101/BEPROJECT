using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Project.Data;
using Project.DTO;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommitteeController : ControllerBase
    {
        private readonly DataContext _context;

        public CommitteeController(DataContext context)
        {
            _context = context;
        }

        // 1. Create multiple committees
        [HttpPost("create")]
        public IActionResult CreateCommittees([FromBody] int numberOfCommittees)
        {
            for (int i = 0; i < numberOfCommittees; i++)
            {
                var committee = new Committee
                {
                    Name = $"Hội đồng {i + 1}",
                    Description = $"Tiểu ban {i + 1}",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };
                _context.Committees.Add(committee);
            }
            _context.SaveChanges();

            return Ok("Committees created successfully.");
        }

        // 2. Add teacher to committee with specific role
        [HttpPost("{committeeId}/add-teacher")]
        public IActionResult AddTeacherToCommittee(int committeeId, [FromBody] TeacherCommitteeRoleDTO dto)
        {
            if (dto == null || dto.TeacherId <= 0)
            {
                return BadRequest("Invalid data.");
            }

            var committee = _context.Committees.Find(committeeId);
            if (committee == null)
            {
                return NotFound("Committee not found.");
            }

            var teacherExists = _context.Teachers.Any(t => t.TeacherID == dto.TeacherId);
            if (!teacherExists)
            {
                return NotFound("Teacher not found.");
            }

            var teacherInCommittee = _context.CommitteeTeacherMembers
                .Any(ctm => ctm.CommitteeID == committeeId && ctm.TeacherID == dto.TeacherId);

            if (teacherInCommittee)
            {
                return Conflict("Teacher is already a member of this committee.");
            }

            var committeeTeacherMember = new CommitteeTeacherMember
            {
                CommitteeID = committeeId,
                TeacherID = dto.TeacherId,
                Role = dto.Role
            };

            _context.CommitteeTeacherMembers.Add(committeeTeacherMember);
            _context.SaveChanges();

            return Ok("Teacher added successfully.");
        }

        // 3. Set number of students for a committee
        [HttpPost("{committeeId}/set-student-count")]
        public IActionResult SetStudentCountForCommittee(int committeeId, [FromBody] int studentCount)
        {
            var committee = _context.Committees.Find(committeeId);
            if (committee == null)
            {
                return NotFound("Committee not found.");
            }

            int totalStudents = _context.Students.Count();
            if (studentCount < 0 || studentCount > totalStudents)
            {
                return BadRequest($"Student count must be between 0 and {totalStudents}.");
            }

            committee.StudentCount = studentCount;
            _context.SaveChanges();

            return Ok("Student count set successfully.");
        }

        // 4. Randomly assign students to committees
        [HttpPost("random-assign-students")]
        public async Task<IActionResult> RandomAssignStudents()
        {
            var committees = await _context.Committees
                .Where(c => c.StudentCount.HasValue)
                .ToListAsync();

            if (!committees.Any())
            {
                return BadRequest("No committees with defined student counts found.");
            }

            var validStudentIDs = await _context.Students
                .Select(s => s.StudentID)
                .ToListAsync();

            var totalStudentCount = validStudentIDs.Count;
            if (totalStudentCount < committees.Sum(c => c.StudentCount.Value))
            {
                return BadRequest("Not enough students available for assignment.");
            }

            var existingAssignments = await _context.CommitteeStudentMembers
                .Select(csm => csm.StudentID)
                .ToListAsync();

            if (existingAssignments.Any())
            {
                _context.CommitteeStudentMembers.RemoveRange(
                    _context.CommitteeStudentMembers.Where(csm => existingAssignments.Contains(csm.StudentID))
                );
                await _context.SaveChangesAsync();
            }

            var random = new Random();
            validStudentIDs = validStudentIDs.OrderBy(_ => random.Next()).ToList();

            var committeeAssignments = new Dictionary<int, List<string>>();
            foreach (var committee in committees)
            {
                committeeAssignments[committee.CommitteeID] = new List<string>();
            }

            int studentIndex = 0;
            foreach (var committee in committees)
            {
                int countToAssign = committee.StudentCount.Value;
                for (int i = 0; i < countToAssign; i++)
                {
                    if (studentIndex >= validStudentIDs.Count)
                    {
                        return BadRequest("Ran out of students to assign.");
                    }

                    var studentId = validStudentIDs[studentIndex++];
                    committeeAssignments[committee.CommitteeID].Add(studentId);
                }
            }

            foreach (var assignment in committeeAssignments)
            {
                foreach (var studentId in assignment.Value)
                {
                    var committeeStudentMember = new CommitteeStudentMember
                    {
                        CommitteeID = assignment.Key,
                        StudentID = studentId
                    };
                    _context.CommitteeStudentMembers.Add(committeeStudentMember);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { AssignedStudents = committeeAssignments.Select(ca => new { CommitteeID = ca.Key, Students = ca.Value }) });
        }

        // 5. Get list of committees
        [HttpGet]
        public async Task<IActionResult> GetCommitteesList()
        {
            var committees = await _context.Committees
                .Include(c => c.CommitteeTeacherMembers)
                .ThenInclude(ctm => ctm.Teacher)
                .Include(c => c.CommitteeStudentMembers)
                .ThenInclude(csm => csm.Student)
                .Select(c => new CommitteeDTO
                {
                    CommitteeID = c.CommitteeID,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedDate = c.CreatedDate,
                    ModifiedDate = c.ModifiedDate,
                    StudentCount = c.StudentCount,
                    Teachers = c.CommitteeTeacherMembers.Select(ctm => new CommitteeTeacherssDTO
                    {
                        TeacherID = ctm.TeacherID,
                        TeacherName = ctm.Teacher.Name,
                        Role = ctm.Role
                    }).ToList(),
                    Students = c.CommitteeStudentMembers.Select(csm => new CommitteeStudentssDTO
                    {
                        StudentID = csm.StudentID,
                        StudentName = csm.Student.Name
                    }).ToList()
                }).ToListAsync();

            return Ok(committees);
        }

        // 6. Get members of a committee
        [HttpGet("{committeeId}/members")]
        public async Task<IActionResult> GetCommitteeMembers(int committeeId)
        {
            var teachers = await _context.CommitteeTeacherMembers
                .Where(ctm => ctm.CommitteeID == committeeId)
                .Select(ctm => new
                {
                    ctm.Teacher.Name,
                    Role = ((TeacherRole)ctm.Role).ToString() // Assuming you have an enum for roles
                })
                .ToListAsync();

            var students = await _context.CommitteeStudentMembers
                .Where(csm => csm.CommitteeID == committeeId)
                .Select(csm => new
                {
                    csm.StudentID,
                    csm.Student.Name,
                    csm.Student.ClassID
                })
                .ToListAsync();

            return Ok(new { Teachers = teachers, Students = students });
        }

        [HttpGet("{committeeId}/details")]
        public async Task<IActionResult> GetCommitteeDetails(int committeeId)
        {
            var committee = await _context.Committees
                .Include(c => c.CommitteeTeacherMembers)
                .ThenInclude(ctm => ctm.Teacher)
                .Include(c => c.CommitteeStudentMembers)
                .ThenInclude(csm => csm.Student)
                .FirstOrDefaultAsync(c => c.CommitteeID == committeeId);

            if (committee == null)
            {
                return NotFound("Committee not found.");
            }

            var committeeDetails = new CommitteeDTO
            {
                CommitteeID = committee.CommitteeID,
                Name = committee.Name,
                Description = committee.Description,
                CreatedDate = committee.CreatedDate,
                ModifiedDate = committee.ModifiedDate,
                StudentCount = committee.StudentCount,
                Teachers = committee.CommitteeTeacherMembers.Select(ctm => new CommitteeTeacherssDTO
                {
                    TeacherID = ctm.Teacher.TeacherID,
                    TeacherName = ctm.Teacher.Name,
                    Role = ctm.Role
                }).ToList(),
                Students = committee.CommitteeStudentMembers.Select(csm => new CommitteeStudentssDTO
                {
                    StudentID = csm.Student.StudentID,
                    StudentName = csm.Student.Name
                }).ToList()
            };

            return Ok(committeeDetails);
        }


        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            // Lấy dữ liệu committees từ database (bao gồm cả giảng viên và sinh viên)
            var committees = await _context.Committees
                .Include(c => c.CommitteeTeacherMembers)
                .ThenInclude(ctm => ctm.Teacher)
                .Include(c => c.CommitteeStudentMembers)
                .ThenInclude(csm => csm.Student)
                .ToListAsync();

            if (committees == null || !committees.Any())
            {
                return NotFound("No data available.");
            }

            using (var package = new ExcelPackage())
            {
                foreach (var committee in committees)
                {
                    // Tạo worksheet mới cho từng committee
                    var worksheet = package.Workbook.Worksheets.Add($"Committee_{committee.Name}");

                    // Thêm tiêu đề
                    worksheet.Cells["A1:D1"].Merge = true;
                    worksheet.Cells["A1"].Value = "DANH SÁCH GIẢNG VIÊN THAM GIA TIỂU BAN CHẤM ĐỒ ÁN TỐT NGHIỆP";
                    worksheet.Cells["A2:D2"].Merge = true;
                    worksheet.Cells["A2"].Value = "HỆ ĐẠI HỌC CHÍNH QUY KHÓA 71 - NGÀNH HỆ THỐNG THÔNG TIN";
                    worksheet.Cells["A3:D3"].Merge = true;
                    worksheet.Cells["A3"].Value = "(Kèm theo Quyết định số: 3934/QĐ-HĐXTN ngày 03/6/2024 Chủ tịch Hội đồng xét tốt nghiệp, trường Đại học Công nghệ GTVT)";
                    worksheet.Cells["A4:D4"].Merge = true;
                    worksheet.Cells["A4"].Value = "I. DANH SÁCH TIỂU BAN";

                    worksheet.Cells["A3"].Value = "(Kèm theo Quyết định số: 3934/QĐ-HĐXTN ngày 03/6/2024 Chủ tịch Hội đồng xét tốt nghiệp, trường Đại học Công nghệ GTVT)";
                    worksheet.Cells["A3:D3"].Merge = true; // Gộp các ô từ A3 đến D3
                    worksheet.Cells["A3:D3"].Style.WrapText = true; // Bật chế độ ngắt dòng tự động
                    worksheet.Cells["A3:D3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Căn giữa theo chiều ngang
                    worksheet.Cells["A3:D3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Căn giữa theo chiều dọc

                    // Tăng chiều cao hàng để có đủ không gian cho 2 dòng
                    worksheet.Row(3).Height = 40; // Bạn có thể tăng giá trị nếu cần nhiều không gian hơn

                    // Đặt chiều rộng cột
                    worksheet.Column(1).Width = 50; // Đặt chiều rộng đủ lớn để tự động ngắt dòng
                    // Style cho tiêu đề
                    worksheet.Cells["A1:A4"].Style.Font.Name = "Times New Roman";
                    worksheet.Cells["A1:A3"].Style.Font.Size = 12;
                    worksheet.Cells["A1,A2,A4"].Style.Font.Bold = true;
                    worksheet.Cells["A3"].Style.Font.Bold = false;
                    worksheet.Cells["A1:A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    // Thêm header cho phần giáo viên
                    worksheet.Cells[5, 1].Value = "STT";
                    worksheet.Cells[5, 2].Value = "Họ và tên";
                    worksheet.Cells[5, 3].Value = "Nhiệm vụ";
                    worksheet.Cells[5, 4].Value = "Đơn vị";
                    worksheet.Cells["A5:D5"].Style.Font.Bold = true;
                    worksheet.Cells["A5:D5"].Style.Font.Name = "Times New Roman";
                    worksheet.Cells["A5:D5"].Style.Font.Size = 12;
                    worksheet.Cells["A5:D5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    // Thêm dữ liệu giáo viên
                    int row = 6;
                    int stt = 1;

                    foreach (var teacher in committee.CommitteeTeacherMembers)
                    {
                        worksheet.Cells[row, 1].Value = stt;
                        worksheet.Cells[row, 2].Value = teacher.Teacher.Name;
                        worksheet.Cells[row, 3].Value = GetRoleName(teacher.Role);
                        worksheet.Cells[row, 4].Value = "Trường Đại học Công nghệ GTVT";

                        worksheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[$"A{row}:D{row}"].Style.Font.Name = "Times New Roman";
                        worksheet.Cells[$"A{row}:D{row}"].Style.Font.Size = 12;

                        row++;
                        stt++;
                    }

                    // Thêm tiêu đề cho phần sinh viên
                    worksheet.Cells[$"A{row + 1}:D{row + 1}"].Merge = true;
                    worksheet.Cells[$"A{row + 1}"].Value = "II.DANH SÁCH SINH VIÊN";
                    worksheet.Cells[$"A{row + 1}:D{row + 1}"].Style.Font.Bold = true;
                    worksheet.Cells[$"A{row + 1}:D{row + 1}"].Style.Font.Name = "Times New Roman";
                    worksheet.Cells[$"A{row + 1}:D{row + 1}"].Style.Font.Size = 12;
                    worksheet.Cells[$"A{row + 1}:D{row + 1}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    // Thêm header cho phần sinh viên
                    worksheet.Cells[row + 2, 1].Value = "STT";
                    worksheet.Cells[row + 2, 2].Value = "Họ và tên";
                    worksheet.Cells[row + 2, 3].Value = "Nhiệm vụ";
                    worksheet.Cells[row + 2, 4].Value = "Đơn vị";
                    worksheet.Cells[$"A{row + 2}:D{row + 2}"].Style.Font.Bold = true;
                    worksheet.Cells[$"A{row + 2}:D{row + 2}"].Style.Font.Name = "Times New Roman";
                    worksheet.Cells[$"A{row + 2}:D{row + 2}"].Style.Font.Size = 12;
                    worksheet.Cells[$"A{row + 2}:D{row + 2}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    row += 3;
                    stt = 1;

                    // Thêm dữ liệu sinh viên
                    foreach (var student in committee.CommitteeStudentMembers)
                    {
                        worksheet.Cells[row, 1].Value = stt;
                        worksheet.Cells[row, 2].Value = student.Student.Name;
                        worksheet.Cells[row, 3].Value = "Sinh viên";
                        worksheet.Cells[row, 4].Value = "Trường Đại học Công nghệ GTVT";

                        worksheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[$"A{row}:D{row}"].Style.Font.Name = "Times New Roman";
                        worksheet.Cells[$"A{row}:D{row}"].Style.Font.Size = 12;

                        row++;
                        stt++;
                    }

                    // Áp dụng viền cho các ô có dữ liệu
                    using (var range = worksheet.Cells[$"A5:D{row - 1}"])
                    {
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                    }

                    // Tự động điều chỉnh kích thước cột cho vừa với nội dung
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }

                // Trả về file Excel dưới dạng download
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"Danh sách hội đồng-{DateTime.Now:yyyyMMdd}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        // Hàm hỗ trợ lấy tên vai trò của giáo viên
        private string GetRoleName(int role)
        {
            return role switch
            {
                0 => "Chủ Tịch",
                1 => "Thư Ký",
                2 => "Ủy Viên",
                _ => "Không Xác Định"
            };
        }


        // 8. Delete a committee
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommittee(int id)
        {
            var committee = await _context.Committees
                .Include(c => c.CommitteeTeacherMembers)
                .Include(c => c.CommitteeStudentMembers)
                .FirstOrDefaultAsync(c => c.CommitteeID == id);

            if (committee == null)
            {
                return NotFound();
            }

            _context.CommitteeTeacherMembers.RemoveRange(committee.CommitteeTeacherMembers);
            _context.CommitteeStudentMembers.RemoveRange(committee.CommitteeStudentMembers);
            _context.Committees.Remove(committee);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Enum for Role (Optional, for better clarity)
        private enum TeacherRole
        {
            Chair = 0,
            Secretary = 1,
            Member = 2
        }
    }
}
