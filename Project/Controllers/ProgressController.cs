using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Project.Models;
using Project.Data;
using Project.DTOs;
using Project.DTO.Request;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        private readonly DataContext _context;

        public ProgressController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Progress
        [HttpPost]
        public async Task<IActionResult> CreateProgress([FromBody] ProgressDTO progressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var progress = new Progress
            {
                Title = progressDto.Title,
                Description = progressDto.Description,
                CreatedUser = "Admin",
                CreatedDate = DateTime.Now,
                ModifiedUser = "Admin",
                ModifiedDate = DateTime.Now,
                StartDate = progressDto.StartDate,  // Use the provided StartDate
                EndDate = progressDto.EndDate       // Use the provided EndDate
            };

            _context.Progresses.Add(progress);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProgressById), new { id = progress.ProgressID }, progress);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgressById(int id)
        {
            var progress = await _context.Progresses.FindAsync(id);

            if (progress == null)
            {
                return NotFound();
            }

            return Ok(progress);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProgresses()
        {
            var progresses = await _context.Progresses.ToListAsync();
            return Ok(progresses);
        }



        // PUT: api/Progress/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProgress(int id, [FromBody] ProgressDTO progressDto)
        {
            if (id != progressDto.ProgressID)
            {
                return BadRequest();
            }

            var progress = await _context.Progresses.FindAsync(id);

            if (progress == null)
            {
                return NotFound();
            }

            // Update the progress fields from the DTO
            progress.Title = progressDto.Title;
            progress.StartDate = progressDto.StartDate;
            progress.EndDate = progressDto.EndDate;
            progress.ModifiedDate = DateTime.Now;  // Set the current date and time
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool ProgressExists(int id)
        {
            return _context.Progresses.Any(e => e.ProgressID == id);
        }

        // DELETE: api/Progress/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var progress = await _context.Progresses.FindAsync(id);
                if (progress == null)
                {
                    return NotFound("Progress not found.");
                }

                _context.Progresses.Remove(progress);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("CheckProgressForToday")]
        public async Task<IActionResult> CheckProgressForToday()
        {
            try
            {
                var currentDate = DateTime.Now.Date;

                var progress = await _context.Progresses
                    .FirstOrDefaultAsync(p => currentDate >= p.StartDate.Date && currentDate <= p.EndDate.Date);

                if (progress == null)
                {
                    return Ok(new { Message = "No progress for today.", Status = false });
                }

                return Ok(new { Message = "Progress is active for today.", Status = true, Progress = progress });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
