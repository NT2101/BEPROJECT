using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Project.Data;
using Project.DTO;
using Project.Interfaces;
using Project.Models;
using Project.DTO.Request;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultiesController : Controller
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public FacultiesController(IFacultyRepository facultyRepository, IMapper mapper, DataContext context)
        {
            _facultyRepository = facultyRepository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetFaculties()
        {
            var faculties = _mapper.Map<List<FacultyDTO>>(_facultyRepository.GetFaculties());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(faculties);
        }

        [HttpGet("{ID}")]
        public IActionResult GetFaculty(String ID)
        {
            if (!_facultyRepository.FacultyExists(ID))
            {
                return NotFound();
            }

            var faculty = _facultyRepository.GetFaculty(ID);

            if (faculty == null)
            {
                return NotFound();
            }

            var facultyDTO = _mapper.Map<FacultyDTO>(faculty);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(facultyDTO);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateFaculty(string id, [FromBody] FacultyRequest updatedFaculty)
        {
            if (updatedFaculty == null)
            {
                return BadRequest("Invalid faculty data.");
            }

            var existingFaculty = _facultyRepository.GetFaculty(id);
            if (existingFaculty == null)
            {
                return NotFound();
            }

            // Map updatedFaculty to existingFaculty
            _mapper.Map(updatedFaculty, existingFaculty);

            // Update additional properties
            existingFaculty.ModifiedDate = DateTime.Now;
            existingFaculty.ModifiedUser = "API";

            try
            {
                if (!_facultyRepository.UpdateFaculty(existingFaculty))
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating faculty: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteFaculty(string id)
        {
            if (!_facultyRepository.FacultyExists(id))
            {
                return NotFound();
            }

            var facultyToDelete = _facultyRepository.GetFaculty(id);

            if (facultyToDelete == null)
            {
                return NotFound();
            }

            // Check if there are any associated Specializations
            if (_facultyRepository.HasAssociatedSpecializations(id))
            {
                return BadRequest("Cannot delete faculty because it has associated specializations.");
            }

            try
            {
                if (!_facultyRepository.DeleteFaculty(facultyToDelete))
                {
                    return StatusCode(500, "Failed to delete faculty.");
                }

                return NoContent(); // Success: Return 204 No Content
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting faculty: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }



        [HttpPost]
        public IActionResult CreateFaculty([FromBody] FacultyDTO facultyDTO)
        {
            if (facultyDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem mã khoa đã tồn tại chưa
            if (_facultyRepository.FacultyExists(facultyDTO.ID))
            {
                ModelState.AddModelError("", "Mã khoa đã tồn tại.");
                return Conflict(ModelState);
            }

            var faculty = _mapper.Map<Faculty>(facultyDTO);
            faculty.CreatedUser = "API";
            faculty.ModifiedUser = "API";
            faculty.CreatedDate = DateTime.Now;
            faculty.ModifiedDate = DateTime.Now;

            if (!_facultyRepository.CreateFaculty(faculty))
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm khoa");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetFaculty), new { id = faculty.ID }, faculty);
        }


    }
}
