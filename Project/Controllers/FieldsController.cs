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
    public class FieldsController : Controller
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public FieldsController(IFieldRepository fieldRepository, IMapper mapper, DataContext context)
        {
            _fieldRepository = fieldRepository;
            _mapper = mapper;
            _context = context;
        }

            [HttpGet]
            public IActionResult GetFields()
            {
                var fields = _mapper.Map<List<FieldDTO>>(_fieldRepository.GetFields());

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(fields);
            }

        [HttpGet("{ID}")]
        public IActionResult GetField(int ID)
        {
            if (!_fieldRepository.FieldExists(ID))
            {
                return NotFound();
            }

            var field = _fieldRepository.GetField(ID);

            if (field == null)
            {
                return NotFound();
            }

            var fieldDTO = _mapper.Map<FieldDTO>(field);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(fieldDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateField(int id, [FromBody] FieldRequest updatedField)
        {
            if (updatedField == null)
            {
                return BadRequest("Invalid field data.");
            }

            var existingField = await _context.Fields.FindAsync(id);
            if (existingField == null)
            {
                return NotFound();
            }

            // Update properties
            existingField.FieldName = updatedField.FieldName;
            existingField.Description = updatedField.Description;
            existingField.ModifiedDate = DateTime.Now;
            existingField.ModifiedUser = "API";

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating field: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }
            [HttpDelete("{id}")]
        public IActionResult DeleteField(int id)
        {
            if (!_fieldRepository.FieldExists(id))
            {
                return NotFound();
            }

            var fieldToDelete = _fieldRepository.GetField(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_fieldRepository.DeleteField(fieldToDelete))
            {
                ModelState.AddModelError("", "Không xóa được lĩnh vực");
            }

            return NoContent();
        }
        [HttpPost]
        public IActionResult CreateField([FromBody] FieldDTO fieldDTO)
        {
            if (fieldDTO == null)
            {
                return BadRequest(ModelState);
            }

            var field = _mapper.Map<Field>(fieldDTO);
            field.CreatedUser = "API";
            field.ModifiedUser = "API";
            field.CreatedDate = DateTime.Now;
            field.ModifiedDate = DateTime.Now;

            if (!_fieldRepository.CreateField(field))
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm lĩnh vực");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetField), new { id = field.ID }, field);
        }
    }
}
