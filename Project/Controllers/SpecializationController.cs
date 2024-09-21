    using Microsoft.AspNetCore.Mvc;
    using AutoMapper;
    using Project.DTO;
    using Project.Interfaces;
    using Project.Models;
    using System;
    using System.Collections.Generic;
    using Project.DTO.Request;
using Project.Repositories;
using NuGet.Protocol.Plugins;


namespace Project.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class SpecializationsController : ControllerBase
        {
            private readonly ISpecializationRepository _specializationRepository;
            private readonly IMapper _mapper;

            public SpecializationsController(ISpecializationRepository specializationRepository, IMapper mapper)
            {
                _specializationRepository = specializationRepository;
                _mapper = mapper;
            }

            // GET: api/Specializations
            [HttpGet]
            public IActionResult GetSpecializations()
            {
                var specializations = _specializationRepository.GetSpecializations();
                var specializationsDTO = _mapper.Map<List<SpecializationDTO>>(specializations);
                return Ok(specializationsDTO);
            }

            // GET: api/Specializations/5
            [HttpGet("{id}")]
            public IActionResult GetSpecialization(String id)
            {
                var specialization = _specializationRepository.GetSpecialization(id);

                if (specialization == null)
                {
                    return NotFound();
                }

                var specializationDTO = _mapper.Map<SpecializationDTO>(specialization);
                return Ok(specializationDTO);
            }

            // POST: api/Specializations
            [HttpPost]
            public IActionResult CreateSpecialization([FromBody] SpecializationDTO specializationDTO)
            {
                if (specializationDTO == null)
                {
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var specialization = _mapper.Map<Specialization>(specializationDTO);
            specialization.CreatedUser = "API";
            specialization.ModifiedUser = "API";
            specialization.CreatedDate = DateTime.Now;
            specialization.ModifiedDate = DateTime.Now;
            if (!_specializationRepository.CreateSpecialization(specialization))
                {
                    ModelState.AddModelError("", "Could not save specialization.");
                    return StatusCode(500, ModelState);
                }

                return CreatedAtAction(nameof(GetSpecialization), new { id = specialization.ID }, specialization);
            }

            // PUT: api/Specializations/5
            [HttpPut("{id}")]
            public IActionResult UpdateSpecialization(String id, [FromBody] SpecializationRequest specializationDTO)
            {

            if (UpdateSpecialization == null)
            {
                return BadRequest("Invalid faculty data.");
            }

            var specializationToUpdate = _specializationRepository.GetSpecialization(id);

                if (specializationToUpdate == null)
                {
                    return NotFound();
                }

                _mapper.Map(specializationDTO, specializationToUpdate);

                if (!_specializationRepository.UpdateSpecialization(specializationToUpdate))
                {
                    ModelState.AddModelError("", "Could not update specialization.");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }

            // DELETE: api/Specializations/5
            [HttpDelete("{id}")]
            public IActionResult DeleteSpecialization(String id)
            {
                var specializationToDelete = _specializationRepository.GetSpecialization(id);

                if (specializationToDelete == null)
                {
                    return NotFound();
                }
            if (_specializationRepository.HasAssociatedClass(id))
            {
                return BadRequest("Cannot delete faculty because it has associated specializations.");
            }

            if (!_specializationRepository.DeleteSpecialization(specializationToDelete))
                {
                    ModelState.AddModelError("", "Could not delete specialization.");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }
        }
    }
