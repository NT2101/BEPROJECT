using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.Interfaces;
using Project.Models;



namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class registrationRequestController : Controller
    {

        
        private readonly DataContext _context;

        [HttpPut("registrationRequests/{requestId}")]
        public ActionResult PutRegistrationRequest(int requestId, RegistrationRequest request)
        {
            try
            {
                // Tìm yêu cầu theo ID
                var existingRequest = _context.RegistrationRequests.Find(requestId);
                if (existingRequest == null)
                {
                    return NotFound("Không tìm thấy yêu cầu đăng ký hướng dẫn này.");
                }

                // Cập nhật trạng thái xác nhận của yêu cầu
                existingRequest.IsConfirmed = request.IsConfirmed;
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xử lý xác nhận yêu cầu đăng ký hướng dẫn: {ex.Message}");
            }
        }
        [HttpPost]
        public ActionResult PostRegistrationRequest(RegistrationRequest request)
        {
            try
            {
                // Kiểm tra xem request có hợp lệ không (ví dụ: đã tồn tại yêu cầu từ sinh viên này chưa?)
                var existingRequest = _context.RegistrationRequests
                    .FirstOrDefault(r => r.StudentId == request.StudentId && r.IsConfirmed == false);

                if (existingRequest != null)
                {
                    return BadRequest("Yêu cầu đăng ký hướng dẫn của sinh viên này đã tồn tại và chưa được xác nhận.");
                }

                // Lưu yêu cầu mới vào cơ sở dữ liệu
                _context.RegistrationRequests.Add(request);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xử lý yêu cầu đăng ký hướng dẫn: {ex.Message}");
            }
        }



    }
}
