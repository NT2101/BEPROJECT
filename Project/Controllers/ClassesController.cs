using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Project.Data;
using Project.DTO;
using Project.Interfaces;
using Project.Models;
using Project.Repositories;
using Project.DTO.Request;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : Controller
    {
        private readonly IClassRepository _classRepository;
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public ClassesController(IClassRepository classRepository, IMapper mapper, DataContext context)
        {
            _classRepository = classRepository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetClasses()
        {
            var classes = _mapper.Map<List<ClassDTO>>(_classRepository.GetClasses());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(classes);
        }

        [HttpGet("{ID}")]
        public IActionResult GetClass(String ID)
        {
            if (!_classRepository.ClassExists(ID))
            {
                return NotFound();
            }

            var classEntity = _classRepository.GetClass(ID);

            if (classEntity == null)
            {
                return NotFound();
            }

            var classDTO = _mapper.Map<ClassDTO>(classEntity);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(classDTO);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateClass(string id, [FromBody] ClassRequest classRequest)
        {
            if (classRequest == null)
            {
                return BadRequest("Invalid class data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var classToUpdate = _context.Classes.FirstOrDefault(c => c.ID == id);

            if (classToUpdate == null)
            {
                return NotFound();
            }

            // Manually map the properties from ClassRequest to Class
            classToUpdate.SpecializationID = classRequest.SpecializationID;
            classToUpdate.ClassName = classRequest.ClassName;
            classToUpdate.Description = classRequest.Description;
            // Map other properties as needed

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Could not update class: {ex.Message}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteClass(string id)
        {
            if (!_classRepository.ClassExists(id))
            {
                return NotFound();
            }

            var classToDelete = _classRepository.GetClass(id);

            if (classToDelete == null)
            {
                return NotFound();
            }

            // Explicitly load the Students collection
            _context.Entry(classToDelete).Collection(c => c.Students).Load();

            // Check if there are any associated students
            if (classToDelete.Students != null && classToDelete.Students.Any())
            {
                return BadRequest("Cannot delete class because it has associated students.");
            }

            try
            {
                if (!_classRepository.DeleteClass(classToDelete))
                {
                    return StatusCode(500, "Failed to delete class.");
                }

                return NoContent(); // Success: Return 204 No Content
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting class: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost]
        public IActionResult CreateClass([FromBody] ClassDTO classRequest)
        {
            if (classRequest == null)
            {
                return BadRequest("Invalid class data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var classEntity = _mapper.Map<Class>(classRequest);
            classEntity.CreatedUser = "API";
            classEntity.ModifiedUser = "API";
            classEntity.CreatedDate = DateTime.Now;
            classEntity.ModifiedDate = DateTime.Now;

            if (!_classRepository.CreateClass(classEntity))
            {
                ModelState.AddModelError("", "Could not save class.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetClass), new { id = classEntity.ID }, classEntity);
        }



    }
}
